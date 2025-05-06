    using System.ComponentModel.DataAnnotations;

    namespace rastreabilidadeAnimais.DTOs;

    /// <summary>
    /// DTO completo para representação de uma Unidade de Exploração
    /// </summary>
    public class UnidadeExploracaoDTO
    {
        /// ID da unidade de exploração
        public int Id { get; set; }

        /// Código da espécie
        public int CodigoEspecie { get; set; }

        /// Quantidade total de animais na UEP
        public int QuantidadeAnimais { get; set; }

        /// Código da propriedade
        public int CodigoPropriedade { get; set; }

        /// Nome da espécie animal 
        [Required(ErrorMessage = "O nome da espécie é obrigatório")]
        public string? NomeEspecie { get; set; }
    }

    /// <summary>
    /// DTO para criação de novas unidades de exploração
    /// </summary>
    public class CreateUnidadeExploracaoDTO
    {
        /// Código da espécie animal inicial 
        [Required(ErrorMessage = "O código da espécie é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Código de espécie inválido")]
        public int CodigoEspecie { get; set; }

        /// Quantidade inicial de animais
        [Required(ErrorMessage = "A quantidade de animais é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa")]
        public int QuantidadeAnimais { get; set; }

        /// Código da propriedade 
        [Required(ErrorMessage = "O código da propriedade é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Código de propriedade inválido")]
        public int CodigoPropriedade { get; set; }
    }

    /// <summary>
    /// DTO para atualização de unidades de exploração existentes
    /// </summary>
    public class UpdateUnidadeExploracaoDTO
    {
        /// Nova quantidade de animais na unidade
        [Required(ErrorMessage = "A quantidade de animais é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa")]
        public int QuantidadeAnimais { get; set; }
    }