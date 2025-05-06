using rastreabilidadeAnimais.Models;

namespace rastreabilidadeAnimais.Repositories.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para operações de persistência de registros de saída de animais
    /// </summary>
    public interface ISaidaAnimaisRepository
    {
        /// Obtém todos os registros de saída de animais 
        Task<IEnumerable<SaidaAnimais>> GetAllAsync();

        /// Obtém um registro específico de saída de animais pelo ID 
        Task<SaidaAnimais?> GetByIdAsync(int id);

        /// Cria um novo registro de saída de animais 
        Task<SaidaAnimais?> CreateAsync(SaidaAnimais saidaAnimais);

        /// Atualiza um registro existente de saída de animais 
        Task<SaidaAnimais?> UpdateAsync(SaidaAnimais saidaAnimais);

        /// Remove um registro de saída de animais pelo ID 
        Task<bool> DeleteAsync(int id);
    }
}