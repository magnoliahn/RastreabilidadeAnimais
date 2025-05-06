using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rastreabilidadeAnimais.Models
{
    /// <summary>
    ///  Classe que representa uma saida no sistema
    /// </summary>
    public class SaidaAnimais
    {
        /// ID da saida
        [Key]
        public int Id { get; set; }

        /// Obtém data e hora da saida
        [Required]
        public DateTime DataSaida { get; set; }

        /// Obtém o codigo da unidade de origem
        [Required]
        public int CodigoUepOrigem { get; set; }

        /// Obtém o codigo da unidade de destino
        [Required]
        public int CodigoUepDestino { get; set; }

        /// Obtém a quantidade de animais que estão saindo
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade de animais deve ser maior que zero.")]
        public int QuantidadeAnimais { get; set; }

        /// <summary>
        /// Propriedade de navegação para a Unidade de Exploração de origem
        /// </summary>
        [ForeignKey("CodigoUepOrigem")]
        public virtual UnidadeExploracao? UepOrigem { get; set; }

        /// <summary>
        /// Propriedade de navegação para a Unidade de Exploração de destino
        /// </summary>
        [ForeignKey("CodigoUepDestino")]
        public virtual UnidadeExploracao? UepDestino { get; set; }
    }
}