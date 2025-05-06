using Microsoft.EntityFrameworkCore;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Repositories;

namespace rastreabilidadeAnimais.Tests.Repositories
{
    /// <summary>
    /// Classe de testes para o UnidadeExploracaoRepository
    /// </summary>
    public class UnidadeExploracaoRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        /// <summary>
        /// Configuração inicial para todos os testes
        /// </summary>
        public UnidadeExploracaoRepositoryTests()
        {
            // Configura o banco de dados em memória
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;
        }

        /// <summary>
        /// Testa se DeleteAsync retorna false quando existem saídas vinculadas à unidade
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenHasLinkedSaidas()
        {
            // Arrange 
            using var context = new ApplicationDbContext(_dbContextOptions);

            // Cria uma UEP
            var uep = new UnidadeExploracao
            {
                Id = 1,
                CodigoEspecie = 1,
                QuantidadeAnimais = 10,
                CodigoPropriedade = 1
            };

            // Cria uma saída vinculada a UEP
            var saida = new SaidaAnimais
            {
                Id = 1,
                CodigoUepOrigem = 1,
                CodigoUepDestino = 2,
                QuantidadeAnimais = 5,
                DataSaida = DateTime.Now
            };

            context.UnidadesExploracao.Add(uep);
            context.SaidasAnimais.Add(saida);
            await context.SaveChangesAsync();

            var repository = new UnidadeExploracaoRepository(context);

            // Act 
            var result = await repository.DeleteAsync(1);

            // Assert 
            Assert.False(result); // Verifica se retornou false (não deletou)
            Assert.NotEmpty(context.UnidadesExploracao); // Verifica se a unidade ainda existe
            Assert.Single(context.UnidadesExploracao); // Confirma que há exatamente 1 unidade
        }
    }
}