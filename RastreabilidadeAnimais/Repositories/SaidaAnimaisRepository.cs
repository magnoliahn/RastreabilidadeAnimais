using Microsoft.EntityFrameworkCore;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Repositories.Interfaces;

namespace rastreabilidadeAnimais.Repositories
{
    /// <summary>
    /// Implementação do repositório para operações de saída de animais
    /// </summary>
    public class SaidaAnimaisRepository : ISaidaAnimaisRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que recebe o contexto via injeção de dependência
        /// </summary>
        public SaidaAnimaisRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as saídas de animais com seus relacionamentos carregados
        /// </summary>
        public async Task<IEnumerable<SaidaAnimais>> GetAllAsync()
        {
            return await _context.SaidasAnimais
                .Include(s => s.UepOrigem!) 
                .ThenInclude(u => u!.Especie) 
                .Include(s => s.UepDestino!) 
                .ThenInclude(u => u!.Especie) 
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma saída específica pelo ID com todos os relacionamentos
        /// </summary>
        public async Task<SaidaAnimais?> GetByIdAsync(int id)
        {
            return await _context.SaidasAnimais
                .Include(s => s.UepOrigem!)
                .ThenInclude(u => u!.Especie)
                .Include(s => s.UepDestino!)
                .ThenInclude(u => u!.Especie)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Cria um novo registro de saída de animais
        /// </summary>
        public async Task<SaidaAnimais?> CreateAsync(SaidaAnimais saidaAnimais)
        {
            _context.SaidasAnimais.Add(saidaAnimais);
            await _context.SaveChangesAsync();
            return saidaAnimais;
        }

        /// <summary>
        /// Atualiza um registro existente de saída de animais
        /// </summary>
        public async Task<SaidaAnimais?> UpdateAsync(SaidaAnimais saidaAnimais)
        {
            _context.Entry(saidaAnimais).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return saidaAnimais;
        }

        /// <summary>
        /// Remove uma saída de animais pelo ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var saida = await _context.SaidasAnimais.FindAsync(id);
            if (saida == null) return false;

            _context.SaidasAnimais.Remove(saida);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}