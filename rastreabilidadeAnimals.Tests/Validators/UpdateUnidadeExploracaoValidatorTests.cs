using FluentValidation.TestHelper; 
using rastreabilidadeAnimais.DTOs;  
using rastreabilidadeAnimais.Validators;  

namespace rastreabilidadeAnimais.Tests.Validators
{
    /// <summary>
    /// Classe de testes para o validador UpdateUnidadeExploracaoValidator
    /// </summary>
    public class UpdateUnidadeExploracaoValidatorTests
    {
        // Instância do validador que será testado
        private readonly UpdateUnidadeExploracaoValidator _validator;

        /// <summary>
        /// Construtor que inicializa o ambiente de teste
        /// </summary>
        public UpdateUnidadeExploracaoValidatorTests()
        {
            _validator = new UpdateUnidadeExploracaoValidator();
        }

        /// <summary>
        /// Testa se o validador retorna erro quando a quantidade de animais é negativa
        /// </summary>
        [Fact]
        public void Should_HaveError_WhenQuantidadeIsNegative()
        {
            // Cria um DTO com quantidade de animais inválida (negativa)
            var dto = new UpdateUnidadeExploracaoDTO { QuantidadeAnimais = -1 };

            // Utiliza o TestValidate do FluentValidation para testar o validador
            _validator.TestValidate(dto)
                .ShouldHaveValidationErrorFor(x => x.QuantidadeAnimais) // Verifica se há erro de validação para a propriedade QuantidadeAnimais
                .WithErrorMessage("A quantidade de animais não pode ser negativa."); // Verifica se a mensagem de erro é a esperada
        }
    }
}