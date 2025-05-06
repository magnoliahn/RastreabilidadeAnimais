using Microsoft.EntityFrameworkCore;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Repositories.Interfaces;

namespace rastreabilidadeAnimais.Repositories
{
    /// <summary>
    /// Implementação do repositório para operações com UEP
    /// </summary>
    public class UnidadeExploracaoRepository : IUnidadeExploracaoRepository
    {
        private readonly ApplicationDbContext _context;

        /// Construtor que inicializa o contexto 
        public UnidadeExploracaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as unidades de exploração cadastradas
        /// </summary>
        public async Task<IEnumerable<UnidadeExploracao>> GetAllAsync()
        {
            return await _context.UnidadesExploracao
                .Include(u => u.Especie)  // Carrega a espécie relacionada
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma unidade de exploração específica pelo ID
        /// </summary>
        public async Task<UnidadeExploracao?> GetByIdAsync(int id)
        {
            return await _context.UnidadesExploracao
                .Include(u => u.Especie)  // Carrega a espécie relacionada
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Cria uma nova unidade de exploração
        /// </summary>
        public async Task<UnidadeExploracao?> CreateAsync(UnidadeExploracao unidadeExploracao)
        {
            _context.UnidadesExploracao.Add(unidadeExploracao);
            await _context.SaveChangesAsync();
            return unidadeExploracao;
        }

        /// <summary>
        /// Atualiza os dados de uma unidade de exploração existente
        /// </summary>
        public async Task<UnidadeExploracao?> UpdateAsync(UnidadeExploracao unidadeExploracao)
        {
            _context.Entry(unidadeExploracao).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return unidadeExploracao;
        }

        /// <summary>
        /// Remove uma unidade de exploração pelo ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var unidade = await _context.UnidadesExploracao.FindAsync(id);
            if (unidade == null) return false;

            // Verifica se existem saídas relacionadas à unidade
            bool hasSaidas = await _context.SaidasAnimais
                .AnyAsync(s => s.CodigoUepOrigem == id || s.CodigoUepDestino == id);

            if (hasSaidas) return false;  // Impede exclusão se houver relacionamentos

            _context.UnidadesExploracao.Remove(unidade);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}