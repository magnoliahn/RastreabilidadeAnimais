using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rastreabilidadeAnimais.Models
{
    /// <summary>
    ///  Classe que representa uma saida no sistema
    /// </summary>
    public class UnidadeExploracao
    {
        /// ID da unidade
        [Key]
        public int Id { get; set; }

        /// Cdigo da especie animal predominante
        [Required]
        public int CodigoEspecie { get; set; }

        /// Quantidade de animais na unidade
        [Required]
        public int QuantidadeAnimais { get; set; }

        /// Codigo da propriedade UEP
        [Required]
        public int CodigoPropriedade { get; set; }

        /// <summary>
        /// Espécie animal associada à unidade de exploração
        /// Relacionamento configurado via chave estrangeira CodigoEspecie
        /// </summary>
        [ForeignKey("CodigoEspecie")]
        public Especie? Especie { get; set; }

        /// <summary>
        /// Lista de saídas de animais que tiveram origem nesta unidade
        /// </summary>
        public virtual ICollection<SaidaAnimais> SaidasOriginadas { get; set; } = new List<SaidaAnimais>();

        /// <summary>
        /// Lista de saídas de animais que tiveram esta unidade como destino 
        /// </summary>
        public virtual ICollection<SaidaAnimais> SaidasDestinadas { get; set; } = new List<SaidaAnimais>();
    }
}