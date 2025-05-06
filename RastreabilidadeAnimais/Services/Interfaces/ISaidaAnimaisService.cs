using rastreabilidadeAnimais.DTOs;

namespace rastreabilidadeAnimais.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato de saídas de animais
    /// </summary>
    public interface ISaidaAnimaisService
    {
        /// <summary>
        /// Obtém todas as saídas de animais registradas no sistema
        /// </summary>
        Task<IEnumerable<SaidaAnimaisDTO>> GetAllAsync();

        /// <summary>
        /// Obtém uma saída de animais específica pelo seu identificador
        /// </summary>
        Task<SaidaAnimaisDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova saída de animais no sistema
        /// </summary>
        Task<SaidaAnimaisDTO?> CreateAsync(CreateSaidaAnimaisDTO dto);

        /// <summary>
        /// Atualiza os dados de uma saída de animais existente
        /// </summary>
        Task<SaidaAnimaisDTO?> UpdateAsync(int id, UpdateSaidaAnimaisDTO dto);

        /// <summary>
        /// Remove um registro de saída de animais do sistema
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}