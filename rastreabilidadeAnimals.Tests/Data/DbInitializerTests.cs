using Microsoft.EntityFrameworkCore;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.Models;

namespace rastreabilidadeAnimais.Tests.Data
{
    /// <summary>
    /// Classe de testes para o DbInitializer
    /// </summary>
    public class DbInitializerTests
    {
        /// <summary>
        /// Testa se o inicializador não adiciona espécies quando já existem no banco
        /// </summary>
        [Fact]
        public void Initialize_ShouldNotAddEspecies_WhenAlreadyExists()
        {
            // Configuração do banco de dados em memória
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid()) // Nome único para cada teste
               .Options;

            // Arrange 
            // Cria um banco com uma espécie já existente
            using (var context = new ApplicationDbContext(options))
            {
                context.Especies.Add(new Especie { Nome = "Bovino" });
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                DbInitializer.Initialize(context); // Chama o inicializador
            }

            // Assert - 
            using (var context = new ApplicationDbContext(options))
            {
                // Verifica se há exatamente uma espécie no banco
                Assert.Single(context.Especies);

                // Verifica se a espécie é a que foi adicionada manualmente
                Assert.Equal("Bovino", context.Especies.First().Nome);
            }
        }
    }
}