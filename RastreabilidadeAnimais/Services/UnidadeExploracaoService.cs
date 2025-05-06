using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Repositories.Interfaces;
using rastreabilidadeAnimais.Services.Interfaces;

namespace rastreabilidadeAnimais.Services
{
    /// <summary>
    /// Serviço para gestão das UEP
    /// </summary>
    public class UnidadeExploracaoService : IUnidadeExploracaoService
    {
        private readonly IUnidadeExploracaoRepository _repository;

        /// <summary>
        /// Construtor com injeção de dependência do repositório
        /// </summary>
        public UnidadeExploracaoService(IUnidadeExploracaoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtém todas as unidades de exploração cadastradas
        /// </summary>
        public async Task<IEnumerable<UnidadeExploracaoDTO>> GetAllAsync()
        {
            var unidades = await _repository.GetAllAsync();
            return unidades.Select(u => MapToDTO(u!));
        }

        /// <summary>
        /// Obtém uma unidade de exploração específica pelo ID
        /// </summary>
        public async Task<UnidadeExploracaoDTO?> GetByIdAsync(int id)
        {
            var unidade = await _repository.GetByIdAsync(id);
            return unidade != null ? MapToDTO(unidade) : null;
        }

        /// <summary>
        /// Cria uma nova unidade de exploração
        /// </summary>
        public async Task<UnidadeExploracaoDTO?> CreateAsync(CreateUnidadeExploracaoDTO dto)
        {
            var unidadeExploracao = new UnidadeExploracao
            {
                CodigoEspecie = dto.CodigoEspecie,
                QuantidadeAnimais = dto.QuantidadeAnimais,
                CodigoPropriedade = dto.CodigoPropriedade
            };

            var unidadeCriada = await _repository.CreateAsync(unidadeExploracao);

            return unidadeCriada != null ? MapToDTO(unidadeCriada) : null;
        }

        /// <summary>
        /// Atualiza a quantidade de animais de uma unidade existente
        /// </summary>
        public async Task<UnidadeExploracaoDTO?> UpdateQuantidadeAsync(int id, UpdateUnidadeExploracaoDTO dto)
        {
            var unidade = await _repository.GetByIdAsync(id);
            if (unidade == null) return null;

            unidade.QuantidadeAnimais = dto.QuantidadeAnimais;
            var updatedUnidade = await _repository.UpdateAsync(unidade);

            return updatedUnidade != null ? MapToDTO(updatedUnidade) : null;
        }

        /// <summary>
        /// Remove uma unidade de exploração pelo ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Método auxiliar para mapeamento de entidade para DTO
        /// </summary>
        private UnidadeExploracaoDTO MapToDTO(UnidadeExploracao unidade)
        {
            if (unidade == null) throw new ArgumentNullException(nameof(unidade));

            return new UnidadeExploracaoDTO
            {
                Id = unidade.Id,
                CodigoEspecie = unidade.CodigoEspecie,
                QuantidadeAnimais = unidade.QuantidadeAnimais,
                CodigoPropriedade = unidade.CodigoPropriedade,
                NomeEspecie = unidade.Especie?.Nome ?? "Desconhecido"
            };
        }
    }
}