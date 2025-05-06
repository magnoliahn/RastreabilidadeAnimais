using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Models;

namespace rastreabilidadeAnimais.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para gestão das UEP
    /// </summary>
    public interface IUnidadeExploracaoService
    {
        /// <summary>
        /// Obtém todas as unidades de exploração cadastradas
        /// </summary>
        Task<IEnumerable<UnidadeExploracaoDTO>> GetAllAsync();

        /// <summary>
        /// Obtém uma unidade de exploração específica pelo seu identificador
        /// </summary>
        Task<UnidadeExploracaoDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova unidade de exploração no sistema
        /// </summary>
        Task<UnidadeExploracaoDTO?> CreateAsync(CreateUnidadeExploracaoDTO dto);

        /// <summary>
        /// Atualiza a quantidade de animais de uma unidade de exploração existente
        /// </summary>
        Task<UnidadeExploracaoDTO?> UpdateQuantidadeAsync(int id, UpdateUnidadeExploracaoDTO dto);

        /// <summary>
        /// Remove uma unidade de exploração do sistema
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}