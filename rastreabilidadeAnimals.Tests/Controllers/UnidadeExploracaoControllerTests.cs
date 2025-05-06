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
    /// Classe de testes para o UnidadeExploracaoController
    /// </summary>
    public class UnidadeExploracaoControllerTests
    {
        private readonly Mock<IUnidadeExploracaoService> _mockService;
        private readonly Mock<IValidator<CreateUnidadeExploracaoDTO>> _mockCreateValidator;
        private readonly Mock<IValidator<UpdateUnidadeExploracaoDTO>> _mockUpdateValidator;
        private readonly UnidadeExploracaoController _controller;

        /// <summary>
        /// Configuração inicial para todos os testes
        /// </summary>
        public UnidadeExploracaoControllerTests()
        {
            // Inicializa os mocks das dependências
            _mockService = new Mock<IUnidadeExploracaoService>();
            _mockCreateValidator = new Mock<IValidator<CreateUnidadeExploracaoDTO>>();
            _mockUpdateValidator = new Mock<IValidator<UpdateUnidadeExploracaoDTO>>();

            // Cria a instância do controller com os mocks
            _controller = new UnidadeExploracaoController(
                _mockService.Object,
                _mockCreateValidator.Object,
                _mockUpdateValidator.Object);
        }

        /// <summary>
        /// Testa se UpdateQuantidade retorna NotFound quando a unidade não existe
        /// </summary>
        [Fact]
        public async Task UpdateQuantidade_ReturnsNotFound_WhenUnidadeDoesNotExist()
        {
            // Arrange 
            var dto = new UpdateUnidadeExploracaoDTO { QuantidadeAnimais = 5 };

            // Configura o validador para retornar sucesso
            _mockUpdateValidator.Setup(x => x.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());

            // Configura o serviço para retornar null (unidade não encontrada)
            _mockService.Setup(x => x.UpdateQuantidadeAsync(1, dto))
                .ReturnsAsync((UnidadeExploracaoDTO)null);

            // Act 
            var result = await _controller.UpdateQuantidade(1, dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<UnidadeExploracaoDTO>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        /// <summary>
        /// Testa se Delete retorna NoContent quando a deleção é bem-sucedida
        /// </summary>
        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            // Configura o serviço para retornar true 
            _mockService.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}