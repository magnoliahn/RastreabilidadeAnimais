using System.ComponentModel.DataAnnotations;
using System.Transactions;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Repositories.Interfaces;
using rastreabilidadeAnimais.Services.Interfaces;

namespace rastreabilidadeAnimais.Services
{
    /// <summary>
    /// Serviço para gestão de saídas de animais entre as UEP
    /// </summary>
    public class SaidaAnimaisService : ISaidaAnimaisService
    {
        private readonly ISaidaAnimaisRepository _saidaRepository;
        private readonly IUnidadeExploracaoRepository _unidadeRepository;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor com injeção de dependências
        /// </summary>
        public SaidaAnimaisService(
            ISaidaAnimaisRepository saidaRepository,
            IUnidadeExploracaoRepository unidadeRepository,
            ApplicationDbContext context)
        {
            _saidaRepository = saidaRepository;
            _unidadeRepository = unidadeRepository;
            _context = context;
        }

        /// <summary>
        /// Obtém todas as saídas de animais cadastradas
        /// </summary>
        public async Task<IEnumerable<SaidaAnimaisDTO>> GetAllAsync()
        {
            var saidas = await _saidaRepository.GetAllAsync();
            return saidas.Select(s => MapToDTO(s!));
        }

        /// <summary>
        /// Obtém uma saída específica pelo ID
        /// </summary>
        public async Task<SaidaAnimaisDTO?> GetByIdAsync(int id)
        {
            var saida = await _saidaRepository.GetByIdAsync(id);
            return saida != null ? MapToDTO(saida) : null;
        }

        /// <summary>
        /// Cria uma nova saída de animais
        /// </summary>
        public async Task<SaidaAnimaisDTO?> CreateAsync(CreateSaidaAnimaisDTO dto)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // Valida unidades de origem e destino
                var uepOrigem = await _unidadeRepository.GetByIdAsync(dto.CodigoUepOrigem);
                if (uepOrigem == null)
                {
                    throw new ValidationException("UEP de origem não encontrada.");
                }

                var uepDestino = await _unidadeRepository.GetByIdAsync(dto.CodigoUepDestino)
                    ?? throw new ValidationException("UEP de destino não encontrada.");

                // Valida compatibilidade de espécies
                if (uepOrigem.CodigoEspecie != uepDestino.CodigoEspecie)
                    throw new ValidationException("UEPs devem ser da mesma espécie.");

                // Cria o registro de saída
                var saida = new SaidaAnimais
                {
                    DataSaida = dto.DataSaida,
                    CodigoUepOrigem = dto.CodigoUepOrigem,
                    CodigoUepDestino = dto.CodigoUepDestino,
                    QuantidadeAnimais = dto.QuantidadeAnimais
                };

                await _saidaRepository.CreateAsync(saida);

                // Atualiza quantidades nas UEPs
                await AtualizarQuantidadesUEPs(
                    uepOrigem.Id,
                    uepDestino.Id,
                    -dto.QuantidadeAnimais,
                    dto.QuantidadeAnimais
                );

                var createdSaida = await _saidaRepository.GetByIdAsync(saida.Id);
                scope.Complete();

                return createdSaida != null ? MapToDTO(createdSaida) : null;
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Falha ao criar saída: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza uma saída existente com validações de negócio
        /// </summary>
        public async Task<SaidaAnimaisDTO?> UpdateAsync(int id, UpdateSaidaAnimaisDTO dto)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var saida = await _saidaRepository.GetByIdAsync(id)
                    ?? throw new ValidationException("Saída não encontrada.");

                var uepOrigem = await _unidadeRepository.GetByIdAsync(saida.CodigoUepOrigem)
                    ?? throw new ValidationException("UEP origem não encontrada.");

                var uepDestino = await _unidadeRepository.GetByIdAsync(saida.CodigoUepDestino)
                    ?? throw new ValidationException("UEP destino não encontrada.");

                // Reverte quantidades antes de atualizar
                await AtualizarQuantidadesUEPs(
                    uepOrigem.Id,
                    uepDestino.Id,
                    saida.QuantidadeAnimais,
                    -saida.QuantidadeAnimais
                );

                // Valida nova quantidade
                if (uepOrigem.QuantidadeAnimais < dto.QuantidadeAnimais)
                    throw new ValidationException("Quantidade insuficiente na origem.");

                // Atualiza a saída
                saida.DataSaida = dto.DataSaida;
                saida.QuantidadeAnimais = dto.QuantidadeAnimais;

                await _saidaRepository.UpdateAsync(saida);

                // Aplica novas quantidades
                await AtualizarQuantidadesUEPs(
                    uepOrigem.Id,
                    uepDestino.Id,
                    -dto.QuantidadeAnimais,
                    dto.QuantidadeAnimais
                );

                scope.Complete();
                return MapToDTO(saida);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Falha ao atualizar: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove uma saída existente 
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var saida = await _saidaRepository.GetByIdAsync(id);
                if (saida == null) return false;

                var uepOrigem = await _unidadeRepository.GetByIdAsync(saida.CodigoUepOrigem);
                var uepDestino = await _unidadeRepository.GetByIdAsync(saida.CodigoUepDestino);

                // Reverte quantidades se unidades existirem
                if (uepOrigem != null && uepDestino != null)
                {
                    await AtualizarQuantidadesUEPs(
                        uepOrigem.Id,
                        uepDestino.Id,
                        saida.QuantidadeAnimais,
                        -saida.QuantidadeAnimais
                    );
                }

                var result = await _saidaRepository.DeleteAsync(id);
                scope.Complete();

                return result;
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Falha ao excluir: {ex.Message}");
            }
        }

        /// <summary>
        /// Método interno para atualização transacional de quantidades nas UEPs
        /// </summary>
        private async Task AtualizarQuantidadesUEPs(int idOrigem, int idDestino, int deltaOrigem, int deltaDestino)
        {
            var uepOrigem = await _context.UnidadesExploracao.FindAsync(idOrigem)
                ?? throw new ValidationException("UEP origem não encontrada.");

            var uepDestino = await _context.UnidadesExploracao.FindAsync(idDestino)
                ?? throw new ValidationException("UEP destino não encontrada.");

            if (deltaOrigem < 0 && uepOrigem.QuantidadeAnimais < -deltaOrigem)
                throw new ValidationException("Quantidade insuficiente na origem.");

            uepOrigem.QuantidadeAnimais += deltaOrigem;
            uepDestino.QuantidadeAnimais += deltaDestino;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Método auxiliar para mapeamento de entidade para DTO
        /// </summary>
        private SaidaAnimaisDTO MapToDTO(SaidaAnimais saida)
        {
            return new SaidaAnimaisDTO
            {
                Id = saida.Id,
                DataSaida = saida.DataSaida,
                CodigoUepOrigem = saida.CodigoUepOrigem,
                CodigoUepDestino = saida.CodigoUepDestino,
                QuantidadeAnimais = saida.QuantidadeAnimais,
                NomeEspecieOrigem = saida.UepOrigem?.Especie?.Nome ?? "Desconhecido",
                NomeEspecieDestino = saida.UepDestino?.Especie?.Nome ?? "Desconhecido"
            };
        }
    }
}