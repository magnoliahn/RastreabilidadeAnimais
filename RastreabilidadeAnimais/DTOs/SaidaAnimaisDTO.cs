using System.ComponentModel.DataAnnotations;

namespace rastreabilidadeAnimais.DTOs
{
    /// <summary>
    /// DTO completo para representação de saída de animais
    /// </summary>
    public class SaidaAnimaisDTO
    {
        /// ID do registro de saída
        public int Id { get; set; }

        /// Data e hora em que ocorreu a saída dos animais
        public DateTime DataSaida { get; set; }

        /// Código da Unidade de Exploração Pecuária de origem
        public int CodigoUepOrigem { get; set; }

        /// Código da Unidade de Exploração Pecuária  de destino
        public int CodigoUepDestino { get; set; }

        /// Quantidade total que sairam
        public int QuantidadeAnimais { get; set; }

        /// Nome da espécie animal na unidade de origem
        [Required(ErrorMessage = "O nome da espécie de origem é obrigatório")]
        public string? NomeEspecieOrigem { get; set; }

        /// Nome da espécie animal na unidade de destino
        [Required(ErrorMessage = "O nome da espécie de destino é obrigatório")]
        public string? NomeEspecieDestino { get; set; }
    }

    /// <summary>
    /// DTO para criação de novos registros de saída de animais
    /// </summary>
    public class CreateSaidaAnimaisDTO
    {
        /// Data e hora em que ocorreu a saída dos animais
        [Required(ErrorMessage = "A data de saída é obrigatória")]
        public DateTime DataSaida { get; set; }

        /// Código da UEP de origem
        [Required(ErrorMessage = "O código da UEP de origem é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Código de origem inválido")]
        public int CodigoUepOrigem { get; set; }

        /// Código da UEP de destino
        [Required(ErrorMessage = "O código da UEP de destino é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Código de destino inválido")]
        public int CodigoUepDestino { get; set; }

        /// Quantidade de animais transferidos
        [Required(ErrorMessage = "A quantidade de animais é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int QuantidadeAnimais { get; set; }
    }

    /// DTO para atualização de registros existentes de saída de animais
    public class UpdateSaidaAnimaisDTO
    {
        /// Nova data para a saída dos animais
        [Required(ErrorMessage = "A data de saída é obrigatória")]
        public DateTime DataSaida { get; set; }

        /// Nova quantidade de animais
        [Required(ErrorMessage = "A quantidade de animais é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int QuantidadeAnimais { get; set; }
    }
}