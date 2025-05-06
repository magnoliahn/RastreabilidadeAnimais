
using rastreabilidadeAnimais.Models;

namespace rastreabilidadeAnimais.Data
{
    /// <summary>
    /// Classe estática responsável pela inicialização do banco de dados em memória
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Método principal que popula as tabelas com dados iniciais
        /// </summary>
        public static void Initialize(ApplicationDbContext context)
        {
            // Garante se as tabelas não existirem, serão criadas
            context.Database.EnsureCreated();

            // Verifica se já existem espécies cadastradas
            // Se existirem, encerra a inicialização para evitar duplicação
            if (context.Especies.Any())
            {
                return;
            }

            // Cria um array de objetos Especie com dados iniciais
            var especies = new Especie[]
            {
                new Especie { Nome = "Bovino" },    
                new Especie { Nome = "Ovino" },     
                new Especie { Nome = "Caprino" },    
                new Especie { Nome = "Suíno" },     
                new Especie { Nome = "Equino"}       
            };

            // Adiciona cada espécie ao contexto 
            foreach (var especie in especies)
            {
                context.Especies.Add(especie);
            }

            // Persiste todas as alterações no banco de dados
            context.SaveChanges();
        }
    }
}