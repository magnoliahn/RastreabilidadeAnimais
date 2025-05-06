using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Services.Interfaces;
using FluentValidation;

namespace rastreabilidadeAnimais.Controllers
{
    /// <summary>
    /// Controller para gerenciar unidades de exploração 
    /// </summary>
    [ApiController] 
    [Route("api/[controller]")] 
    public class UnidadeExploracaoController : ControllerBase // Herda da classe base para controllers API
    {
        // Dependencias
        private readonly IUnidadeExploracaoService _service;
        private readonly IValidator<CreateUnidadeExploracaoDTO> _createValidator;
        private readonly IValidator<UpdateUnidadeExploracaoDTO> _updateValidator;

        /// <summary>
        /// Construtor com injeção de dependências
        /// </summary>
        public UnidadeExploracaoController(
            IUnidadeExploracaoService service,
            IValidator<CreateUnidadeExploracaoDTO> createValidator,
            IValidator<UpdateUnidadeExploracaoDTO> updateValidator)
        {
            // Inicializa as dependências
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Obtém todas as unidades de exploração registradas
        /// </summary>
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<UnidadeExploracaoDTO>>> GetAll()
        {
            // Obtém as unidades através do serviço
            var unidades = await _service.GetAllAsync();
            // Retorna 200 com a lista de unidades
            return Ok(unidades);
        }

        /// <summary>
        /// Obtém uma unidade de exploração especifica por ID
        /// </summary>
        [HttpGet("{id}")] 
        public async Task<ActionResult<UnidadeExploracaoDTO>> GetById(int id)
        {
            // Busca pelo ID
            var unidade = await _service.GetByIdAsync(id);
            // Se não encontrada, retorna 404 
            if (unidade == null)
                return NotFound();

            // Se encontrada, retorna 200 
            return Ok(unidade);
        }

        /// <summary>
        /// Cria uma nova unidade de exploração
        /// </summary>
        [HttpPost] 
        public async Task<ActionResult<UnidadeExploracaoDTO>> Create(CreateUnidadeExploracaoDTO dto)
        {
            // Valida o DTO recebido
            ValidationResult validationResult = await _createValidator.ValidateAsync(dto);
            // Se inválido, retorna 400 + erros
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

            try
            {
                // Cria a unidade
                var createdUnidade = await _service.CreateAsync(dto);
                // Se falhar, retorna 400 
                if (createdUnidade == null) return BadRequest("Falha ao criar unidade");

                // Se sucesso, retorna 201 
                return CreatedAtAction(nameof(GetById), new { id = createdUnidade.Id }, createdUnidade);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna 400 + mensagem
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza a quantidade de uma unidade de exploração pleo ID
        /// </summary>
        [HttpPut("{id}")] 
        public async Task<ActionResult<UnidadeExploracaoDTO>> UpdateQuantidade(int id, UpdateUnidadeExploracaoDTO dto)
        {
            // Valida o DTO recebido
            ValidationResult validationResult = await _updateValidator.ValidateAsync(dto);
            // Se inválido, retorna 400 + mensagem de erro
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            try
            {
                // Atualiza a unidade
                var updatedUnidade = await _service.UpdateQuantidadeAsync(id, dto);
                // Se não encontrada, retorna 404
                if (updatedUnidade == null)
                    return NotFound();

                // Se sucesso, retorna 200  com os dados atualizados
                return Ok(updatedUnidade);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna 400 + mensagem
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove uma unidade de exploração pelo ID
        /// </summary>
        [HttpDelete("{id}")] 
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Deleta a unidade
                bool result = await _service.DeleteAsync(id);
                // Se não encontrada, retorna 404
                if (!result)
                    return NotFound();

                // Se sucesso, retorna 204 
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}