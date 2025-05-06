using Microsoft.EntityFrameworkCore;
using rastreabilidadeAnimais.Models;

namespace rastreabilidadeAnimais.Data
{
    /// <summary>
    /// Classe principal que representa o contexto do banco de dados na aplicação.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Construtor sem parâmetros para testes unitários
        /// </summary>
        public ApplicationDbContext() { }

        /// <summary>
        /// Construtor principal que recebe as opções de configuração
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// DbSet da tabela espécie
        public virtual DbSet<Especie> Especies { get; set; }

        /// DbSet da tabela Unidades de Exploração
        public virtual DbSet<UnidadeExploracao> UnidadesExploracao { get; set; }

        /// DbSet da tabela Saídas de Animais
        public virtual DbSet<SaidaAnimais> SaidasAnimais { get; set; }

        /// <summary>
        /// Fluent API para configuração dos models
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chama a implementação base para manter comportamentos padrão
            base.OnModelCreating(modelBuilder);

            // Configuração do relacionamento entre SaidaAnimais e UnidadeExploracao (origem)
            modelBuilder.Entity<SaidaAnimais>()
                .HasOne(s => s.UepOrigem)
                .WithMany(u => u.SaidasOriginadas)
                .HasForeignKey(s => s.CodigoUepOrigem)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento entre SaidaAnimais e UnidadeExploracao (destino)
            modelBuilder.Entity<SaidaAnimais>()
                .HasOne(s => s.UepDestino)
                .WithMany(u => u.SaidasDestinadas)
                .HasForeignKey(s => s.CodigoUepDestino)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}