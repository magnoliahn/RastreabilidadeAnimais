using rastreabilidadeAnimais.Models;

namespace rastreabilidadeAnimais.Repositories.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para operações de persistência das UEP
    /// </summary>
    public interface IUnidadeExploracaoRepository
    {
        /// Obtém todas as unidades de exploração cadastradas
        Task<IEnumerable<UnidadeExploracao>> GetAllAsync();

        /// Obtém uma unidade de exploração específica pelo ID
        Task<UnidadeExploracao?> GetByIdAsync(int id);

        /// Cria uma nova UEP no repositório
        Task<UnidadeExploracao?> CreateAsync(UnidadeExploracao unidadeExploracao);

        /// Atualiza os dados de uma UEP existente
        Task<UnidadeExploracao?> UpdateAsync(UnidadeExploracao unidadeExploracao);

        /// Remove uma UEP pelo ID
        Task<bool> DeleteAsync(int id);
    }
}