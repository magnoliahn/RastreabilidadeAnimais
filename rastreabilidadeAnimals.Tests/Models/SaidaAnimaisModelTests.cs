using System.ComponentModel.DataAnnotations;
using rastreabilidadeAnimais.Models;

namespace rastreabilidadeAnimais.Tests.Models
{
    /// <summary>
    /// Classe de testes para validação do modelo SaidaAnimais
    /// </summary>
    public class SaidaAnimaisModelTests
    {
        /// <summary>
        /// Testa se a propriedade QuantidadeAnimais possui o atributo Range configurado corretamente
        /// </summary>
        [Fact]
        public void QuantidadeAnimais_ShouldHaveRangeAttribute()
        {
            // Arrange
            // Obtém a informação da propriedade QuantidadeAnimais usando reflection
            var property = typeof(SaidaAnimais).GetProperty("QuantidadeAnimais");

            // Obtém o atributo Range da propriedade
            var attribute = property?.GetCustomAttributes(typeof(RangeAttribute), true)
                .FirstOrDefault() as RangeAttribute;

            // Assert (Verificação)
            // Verifica se o atributo Range existe na propriedade
            Assert.NotNull(attribute);

            // Verifica se o valor mínimo está configurado como 1
            Assert.Equal(1, attribute?.Minimum);

            // Verifica se o valor máximo está configurado como int.MaxValue
            Assert.Equal(int.MaxValue, attribute?.Maximum);
        }
    }
}