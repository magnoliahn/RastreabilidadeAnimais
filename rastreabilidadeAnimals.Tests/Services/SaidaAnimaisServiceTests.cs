using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Moq;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Repositories.Interfaces;
using rastreabilidadeAnimais.Services;

namespace rastreabilidadeAnimais.Tests.Services
{
    /// <summary>
    /// Classe de testes para o SaidaAnimaisService
    /// </summary>
    public class SaidaAnimaisServiceTests 
    {
        private readonly Mock<ISaidaAnimaisRepository> _mockSaidaRepo;
        private readonly Mock<IUnidadeExploracaoRepository> _mockUepRepo;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly SaidaAnimaisService _service;

        /// <summary>
        /// Configuração inicial para todos os testes
        /// </summary>
        public SaidaAnimaisServiceTests()
        {
            // Inicializa os mocks das dependências
            _mockSaidaRepo = new Mock<ISaidaAnimaisRepository>();
            _mockUepRepo = new Mock<IUnidadeExploracaoRepository>();
            _mockContext = new Mock<ApplicationDbContext>();

            // Cria a instância do serviço com os mocks
            _service = new SaidaAnimaisService(
                _mockSaidaRepo.Object,
                _mockUepRepo.Object,
                _mockContext.Object);
        }

        /// <summary>
        /// Testa a criação de uma saída de animais
        /// </summary>
        [Fact]
        public async Task CreateAsync_ShouldCreate_WhenValid()
        {
            // Arrange (
            var uepOrigem = new UnidadeExploracao
            {
                Id = 1,
                QuantidadeAnimais = 10,
                CodigoEspecie = 1
            };

            var uepDestino = new UnidadeExploracao
            {
                Id = 2,
                QuantidadeAnimais = 5,
                CodigoEspecie = 1
            };

            // Configura os mocks para retornar as UEP
            _mockUepRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(uepOrigem);
            _mockUepRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(uepDestino);

            var saida = new SaidaAnimais
            {
                Id = 1,
                CodigoUepOrigem = 1,
                CodigoUepDestino = 2,
                QuantidadeAnimais = 3
            };

            // Configura o mock do repositório de saídas
            _mockSaidaRepo.Setup(x => x.CreateAsync(It.IsAny<SaidaAnimais>()))
                .Callback<SaidaAnimais>(s => s.Id = 1)
                .ReturnsAsync((SaidaAnimais s) => s);

            _mockSaidaRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(saida);

            // Configura o mock do contexto
            var mockDbSet = new Mock<DbSet<UnidadeExploracao>>();
            _mockContext.Setup(x => x.UnidadesExploracao).Returns(mockDbSet.Object);
            _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
            _mockContext.Setup(c => c.UnidadesExploracao.FindAsync(1)).ReturnsAsync(uepOrigem);
            _mockContext.Setup(c => c.UnidadesExploracao.FindAsync(2)).ReturnsAsync(uepDestino);

            var dto = new CreateSaidaAnimaisDTO
            {
                CodigoUepOrigem = 1,
                CodigoUepDestino = 2,
                QuantidadeAnimais = 3,
                DataSaida = DateTime.Now
            };

            // Act 
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        /// <summary>
        /// Testa se lança exceção quando origem e destino são iguais
        /// </summary>
        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenSameOriginDestination()
        {
            // Arrange
            var dto = new CreateSaidaAnimaisDTO
            {
                CodigoUepOrigem = 1,
                CodigoUepDestino = 1, // Mesmo código para origem e destino
                QuantidadeAnimais = 3,
                DataSaida = DateTime.Now
            };

            // Act e Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(dto));
        }

        /// <summary>
        /// Testa se lança exceção quando as espécies são diferentes
        /// </summary>
        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDifferentSpecies()
        {
            // Arrange
            var uepOrigem = new UnidadeExploracao { Id = 1, CodigoEspecie = 1 };
            var uepDestino = new UnidadeExploracao { Id = 2, CodigoEspecie = 2 }; // Espécie diferente

            _mockUepRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(uepOrigem);
            _mockUepRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(uepDestino);
            _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var dto = new CreateSaidaAnimaisDTO
            {
                CodigoUepOrigem = 1,
                CodigoUepDestino = 2,
                QuantidadeAnimais = 1,
                DataSaida = DateTime.Now
            };

            // Act e Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(dto));
        }

        /// <summary>
        /// Testa a atualização de uma saída de animais
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ShouldUpdate_WhenValid()
        {
            // Arrange
            var saidaExistente = new SaidaAnimais
            {
                Id = 1,
                CodigoUepOrigem = 1,
                CodigoUepDestino = 2,
                QuantidadeAnimais = 2
            };

            var uepOrigem = new UnidadeExploracao { Id = 1, QuantidadeAnimais = 10 };
            var uepDestino = new UnidadeExploracao { Id = 2, QuantidadeAnimais = 10 };

            // Configura os mocks
            _mockSaidaRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(saidaExistente);
            _mockUepRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(uepOrigem);
            _mockUepRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(uepDestino);

            _mockSaidaRepo.Setup(x => x.UpdateAsync(It.IsAny<SaidaAnimais>()))
                .ReturnsAsync(new SaidaAnimais { Id = 1, QuantidadeAnimais = 3 });

            _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Configura o mock do DbSet
            var mockUepDbSet = new Mock<DbSet<UnidadeExploracao>>();
            _mockContext.Setup(x => x.UnidadesExploracao).Returns(mockUepDbSet.Object);
            _mockContext.Setup(c => c.UnidadesExploracao.FindAsync(1)).ReturnsAsync(uepOrigem);
            _mockContext.Setup(c => c.UnidadesExploracao.FindAsync(2)).ReturnsAsync(uepDestino);

            var dto = new UpdateSaidaAnimaisDTO
            {
                QuantidadeAnimais = 3,
                DataSaida = DateTime.Now
            };

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.QuantidadeAnimais);
        }
    }
}