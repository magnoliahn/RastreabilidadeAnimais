using Microsoft.AspNetCore.Mvc;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Services.Interfaces;
using FluentValidation;
namespace rastreabilidadeAnimais.Controllers
{
    /// <summary>
    /// Controller que gerencia as saidas de animais
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] 
    public class SaidaAnimaisController : ControllerBase // Herda da classe base para controllers API
    {
        // Dependencias
        private readonly ISaidaAnimaisService _service; 
        private readonly IValidator<CreateSaidaAnimaisDTO> _createValidator;
        private readonly IValidator<UpdateSaidaAnimaisDTO> _updateValidator;

        /// <summary>
        /// Construtor com injeção de dependências
        /// </summary>
        public SaidaAnimaisController(
            ISaidaAnimaisService service,
            IValidator<CreateSaidaAnimaisDTO> createValidator,
            IValidator<UpdateSaidaAnimaisDTO> updateValidator)
        {
            // Inicializa as dependências
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Endpoint GET que obtem todas as saídas de animais
        /// </summary>
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<SaidaAnimaisDTO>>> GetAll()
        {
            // Chama o serviço para obter todas as saídas
            var saidas = await _service.GetAllAsync();
            // Retorna 200  com a lista de saídas
            return Ok(saidas);
        }

        /// <summary>
        /// Endpoint GET que obtem saída por um id especifico 
        /// </summary>
        [HttpGet("{id}")] 
        public async Task<ActionResult<SaidaAnimaisDTO>> GetById(int id)
        {
            // Chama o serviço para obter a saída pelo ID
            var saida = await _service.GetByIdAsync(id);
            // Retorna 200 se encontrou, 404  se não encontrou
            return saida != null ? Ok(saida) : NotFound();
        }

        /// <summary>
        /// Endpoint POST cria uma nova saída de animal
        /// </summary>
        [HttpPost] 
        public async Task<ActionResult<SaidaAnimaisDTO>> Create(CreateSaidaAnimaisDTO dto)
        {
            // Valida o DTO recebido
            var validation = await _createValidator.ValidateAsync(dto);
            // Se for inválido retorna 400 + mensagem
            if (!validation.IsValid) return BadRequest(validation.Errors);

            try
            {
                // Chama o serviço que cria a saída
                var createdSaida = await _service.CreateAsync(dto);
                // se criado, retorna 201 com a localização do novo recurso
                // Se falhar retorna 400 
                return createdSaida != null
                    ? CreatedAtAction(nameof(GetById), new { id = createdSaida.Id }, createdSaida)
                    : BadRequest("Falha ao criar saída");
            }
            catch (Exception ex)
            {
                // Em caso de erro retorna 400 + mensagem
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint PUT para atualizar uma saída 
        /// </summary>
        [HttpPut("{id}")] 
        public async Task<ActionResult<SaidaAnimaisDTO>> Update(int id, UpdateSaidaAnimaisDTO dto)
        {
            // Valida o DTO recebido
            var validation = await _updateValidator.ValidateAsync(dto);
            // Se inválido, retorna 400 Bad Request + mensagem
            if (!validation.IsValid) return BadRequest(validation.Errors);

            try
            {
                // Chama o serviço que atualiza a saída
                var updatedSaida = await _service.UpdateAsync(id, dto);
                // Se encontrado e atualizado, retorna 200 com os dados atualizados
                // Se não encontrado, retorna 404 
                return updatedSaida != null
                    ? Ok(updatedSaida)
                    : NotFound();
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint DELETE para remover uma saída
        /// </summary>
        [HttpDelete("{id}")] 
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Chama o serviço que deleta saída
                var result = await _service.DeleteAsync(id);
                // Se deletado com sucesso, retorna 204 
                // Se não encontrado, retorna 404 
                return result ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}