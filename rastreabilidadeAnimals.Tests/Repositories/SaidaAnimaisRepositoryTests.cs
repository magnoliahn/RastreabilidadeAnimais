using Microsoft.EntityFrameworkCore;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Repositories;

namespace rastreabilidadeAnimais.Tests.Repositories
{
    /// <summary>
    /// Classe de testes para o SaidaAnimaisRepository
    /// </summary>
    public class SaidaAnimaisRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        /// <summary>
        /// Configuração inicial para todos os testes
        /// </summary>
        public SaidaAnimaisRepositoryTests()
        {
            // Configura um banco de dados em memória 
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_SaidaAnimais" + Guid.NewGuid())
                .Options;
        }

        /// <summary>
        /// Testa se CreateAsync adiciona corretamente uma entidade
        /// </summary>
        [Fact]
        public async Task CreateAsync_AddsEntityToDatabase()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            var repository = new SaidaAnimaisRepository(context);
            var saida = new SaidaAnimais
            {
                Id = 1,
                QuantidadeAnimais = 10,
                // Outras propriedades obrigatórias
                DataSaida = DateTime.Now,
                CodigoUepOrigem = 1,
                CodigoUepDestino = 2
            };

            // Act 
            var result = await repository.CreateAsync(saida);

            // Assert (Verificação)
            Assert.Equal(1, result.Id); // Verifica se retornou a entidade com ID correto
            Assert.Single(context.SaidasAnimais); // Verifica se há exatamente 1 registro
            Assert.Equal(10, context.SaidasAnimais.First().QuantidadeAnimais); // Verifica os dados salvos
        }

        /// <summary>
        /// Testa se DeleteAsync remove corretamente uma entidade do banco de dados
        /// </summary>
        [Fact]
        public async Task DeleteAsync_RemovesEntityFromDatabase()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);

            // Adiciona um registro de teste
            context.SaidasAnimais.Add(new SaidaAnimais
            {
                Id = 1,
                DataSaida = DateTime.Now,
                CodigoUepOrigem = 1,
                CodigoUepDestino = 2,
                QuantidadeAnimais = 5
            });
            await context.SaveChangesAsync();

            var repository = new SaidaAnimaisRepository(context);

            // Act
            var result = await repository.DeleteAsync(1);

            // Assert
            Assert.True(result); // Verifica se retornou true 
            Assert.Empty(context.SaidasAnimais); // Verifica se a tabela está vazia
        }
    }
}