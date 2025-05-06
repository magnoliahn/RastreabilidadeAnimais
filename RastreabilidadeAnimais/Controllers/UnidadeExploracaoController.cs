using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Services.Interfaces;
using FluentValidation;

namespace rastreabilidadeAnimais.Controllers
{
    /// <summary>
    /// Controller para gerenciar unidades de explora��o 
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
        /// Construtor com inje��o de depend�ncias
        /// </summary>
        public UnidadeExploracaoController(
            IUnidadeExploracaoService service,
            IValidator<CreateUnidadeExploracaoDTO> createValidator,
            IValidator<UpdateUnidadeExploracaoDTO> updateValidator)
        {
            // Inicializa as depend�ncias
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Obt�m todas as unidades de explora��o registradas
        /// </summary>
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<UnidadeExploracaoDTO>>> GetAll()
        {
            // Obt�m as unidades atrav�s do servi�o
            var unidades = await _service.GetAllAsync();
            // Retorna 200 com a lista de unidades
            return Ok(unidades);
        }

        /// <summary>
        /// Obt�m uma unidade de explora��o especifica por ID
        /// </summary>
        [HttpGet("{id}")] 
        public async Task<ActionResult<UnidadeExploracaoDTO>> GetById(int id)
        {
            // Busca pelo ID
            var unidade = await _service.GetByIdAsync(id);
            // Se n�o encontrada, retorna 404 
            if (unidade == null)
                return NotFound();

            // Se encontrada, retorna 200 
            return Ok(unidade);
        }

        /// <summary>
        /// Cria uma nova unidade de explora��o
        /// </summary>
        [HttpPost] 
        public async Task<ActionResult<UnidadeExploracaoDTO>> Create(CreateUnidadeExploracaoDTO dto)
        {
            // Valida o DTO recebido
            ValidationResult validationResult = await _createValidator.ValidateAsync(dto);
            // Se inv�lido, retorna 400 + erros
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
        /// Atualiza a quantidade de uma unidade de explora��o pleo ID
        /// </summary>
        [HttpPut("{id}")] 
        public async Task<ActionResult<UnidadeExploracaoDTO>> UpdateQuantidade(int id, UpdateUnidadeExploracaoDTO dto)
        {
            // Valida o DTO recebido
            ValidationResult validationResult = await _updateValidator.ValidateAsync(dto);
            // Se inv�lido, retorna 400 + mensagem de erro
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            try
            {
                // Atualiza a unidade
                var updatedUnidade = await _service.UpdateQuantidadeAsync(id, dto);
                // Se n�o encontrada, retorna 404
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
        /// Remove uma unidade de explora��o pelo ID
        /// </summary>
        [HttpDelete("{id}")] 
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Deleta a unidade
                bool result = await _service.DeleteAsync(id);
                // Se n�o encontrada, retorna 404
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