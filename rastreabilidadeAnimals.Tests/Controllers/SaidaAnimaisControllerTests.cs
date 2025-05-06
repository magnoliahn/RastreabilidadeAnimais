using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using rastreabilidadeAnimais.Controllers;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Services.Interfaces;
using rastreabilidadeAnimais.Validators;

namespace rastreabilidadeAnimais.Tests.Controllers
{
    /// <summary>
    /// Classe de testes para o SaidaAnimaisController
    /// </summary>
    public class SaidaAnimaisControllerTests
    {
        private readonly Mock<ISaidaAnimaisService> _mockService;
        private readonly Mock<IValidator<CreateSaidaAnimaisDTO>> _mockCreateValidator;
        private readonly Mock<IValidator<UpdateSaidaAnimaisDTO>> _mockUpdateValidator;
        private readonly SaidaAnimaisController _controller;

        /// <summary>
        /// Configuração inicial para todos os testes
        /// </summary>
        public SaidaAnimaisControllerTests()
        {
            // Cria mocks das dependências
            _mockService = new Mock<ISaidaAnimaisService>();
            _mockCreateValidator = new Mock<IValidator<CreateSaidaAnimaisDTO>>();
            _mockUpdateValidator = new Mock<IValidator<UpdateSaidaAnimaisDTO>>();

            // Instancia o controller com os mocks
            _controller = new SaidaAnimaisController(
                _mockService.Object,
                _mockCreateValidator.Object,
                _mockUpdateValidator.Object);
        }

        /// <summary>
        /// Testa se GetAll retorna OkResult com lista de saídas
        /// </summary>
        [Fact]
        public async Task GetAll_ReturnsOkResultWithSaidas()
        {
            // Arrange (Preparação)
            var mockService = new Mock<ISaidaAnimaisService>();
            mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<SaidaAnimaisDTO>());

            var controller = new SaidaAnimaisController(
                mockService.Object,
                Mock.Of<IValidator<CreateSaidaAnimaisDTO>>(),
                Mock.Of<IValidator<UpdateSaidaAnimaisDTO>>()
            );

            // Act (Ação)
            var result = await controller.GetAll();

            // Assert (Verificação)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<SaidaAnimaisDTO>>(okResult.Value);
        }

        /// <summary>
        /// Testa se GetById retorna NotFound quando a saída não existe
        /// </summary>
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenSaidaDoesNotExist()
        {
            // Arrange
            _mockService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((SaidaAnimaisDTO)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Testa se Create retorna BadRequest quando a validação falha
        /// </summary>
        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var dto = new CreateSaidaAnimaisDTO();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Property", "Error")
            });

            _mockCreateValidator.Setup(x => x.ValidateAsync(dto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SaidaAnimaisDTO>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);
        }

        /// <summary>
        /// Testa se Create retorna Created quando os dados são válidos
        /// </summary>
        [Fact]
        public async Task Create_ReturnsCreated_WhenValid()
        {
            // Arrange
            var dto = new CreateSaidaAnimaisDTO();
            var createdSaida = new SaidaAnimaisDTO { Id = 1 };

            _mockCreateValidator.Setup(x => x.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());

            _mockService.Setup(x => x.CreateAsync(dto))
                .ReturnsAsync(createdSaida);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SaidaAnimaisDTO>>(result);
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

            // Verifica se a ação de redirecionamento está correta
            Assert.Equal(nameof(SaidaAnimaisController.GetById), createdAtResult.ActionName);

            // Verifica se o ID está correto nos valores da rota
            Assert.Equal(createdSaida.Id, createdAtResult.RouteValues["id"]);
        }
    }
}