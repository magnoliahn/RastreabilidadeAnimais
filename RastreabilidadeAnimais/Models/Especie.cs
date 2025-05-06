using System.ComponentModel.DataAnnotations;

namespace rastreabilidadeAnimais.Models
{
    /// Classe que representa uma especie no sistema
    public class Especie
    {
        /// ID da especie
        [Key]
        public int Id { get; set; }

        /// Nome da especie
        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
    }
}
