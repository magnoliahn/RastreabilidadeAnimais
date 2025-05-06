using Moq;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Repositories.Interfaces;
using rastreabilidadeAnimais.Services;

namespace rastreabilidadeAnimais.Tests.Services
{
    /// <summary>
    /// Classe de testes para o UnidadeExploracaoService
    /// </summary>
    public class UnidadeExploracaoServiceTests
    {
        private readonly Mock<IUnidadeExploracaoRepository> _mockRepo;
        private readonly UnidadeExploracaoService _service;

        /// <summary>
        /// Configuração inicial para todos os testes
        /// </summary>
        public UnidadeExploracaoServiceTests()
        {
            // Inicializa o mock do repositório
            _mockRepo = new Mock<IUnidadeExploracaoRepository>();

            // Cria a instância do serviço com o mock do repositório
            _service = new UnidadeExploracaoService(_mockRepo.Object);
        }

        /// <summary>
        /// Testa a criação de uma UEP
        /// </summary>
        [Fact]
        public async Task CreateAsync_ShouldCreate_WhenValid()
        {
            // Arrange
            var uep = new UnidadeExploracao
            {
                Id = 1,
                CodigoEspecie = 1,
                QuantidadeAnimais = 10
            };

            // Configura o mock para retornar a uep criada
            _mockRepo.Setup(x => x.CreateAsync(It.IsAny<UnidadeExploracao>()))
                .ReturnsAsync(uep);

            // Configura o mock para retornar a unidade ao buscar por ID
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(uep);

            var dto = new CreateUnidadeExploracaoDTO
            {
                CodigoEspecie = 1,
                QuantidadeAnimais = 10,
                CodigoPropriedade = 1
            };

            // Act 
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result); // Verifica se o resultado não é nulo
            Assert.Equal(1, result.Id); // Verifica se o ID retornado está correto

            // Verifica se o método do repositório foi chamado uma vez
            _mockRepo.Verify(x => x.CreateAsync(It.IsAny<UnidadeExploracao>()), Times.Once);
        }

        /// <summary>
        /// Testa a atualização da quantidade de animais
        /// </summary>
        [Fact]
        public async Task UpdateQuantidadeAsync_ShouldUpdate_WhenValid()
        {
            // Arrange
            var uep = new UnidadeExploracao { Id = 1, QuantidadeAnimais = 5 };
            var updatedUep = new UnidadeExploracao { Id = 1, QuantidadeAnimais = 10 };

            // Configura os mocks
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(uep);
            _mockRepo.Setup(x => x.UpdateAsync(It.IsAny<UnidadeExploracao>()))
                .ReturnsAsync(updatedUep);

            var dto = new UpdateUnidadeExploracaoDTO { QuantidadeAnimais = 10 };

            // Act
            var result = await _service.UpdateQuantidadeAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.QuantidadeAnimais); // Verifica se a quantidade foi atualizada

            // Verifica se o método de atualização foi chamado uma vez
            _mockRepo.Verify(x => x.UpdateAsync(It.IsAny<UnidadeExploracao>()), Times.Once);
        }

        /// <summary>
        /// Testa a falha na deleção quando há saídas vinculadas
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenHasLinkedSaidas()
        {
            // Arrange
            // Configura o mock para simular falha na deleção, possivelmente por ter saídas vinculadas
            _mockRepo.Setup(x => x.DeleteAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result); // Verifica se retornou false 
        }

        /// <summary>
        /// Testa o retorno nulo ao buscar uma unidade inexistente
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            // Configura o mock para retornar null 
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((UnidadeExploracao)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Null(result); // Verifica se o resultado é nulo como esperado
        }
    }
}