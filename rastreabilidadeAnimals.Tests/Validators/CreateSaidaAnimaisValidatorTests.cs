using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.DTOs;
using rastreabilidadeAnimais.Models;
using rastreabilidadeAnimais.Validators;

namespace rastreabilidadeAnimais.Tests.Validators
{
    /// <summary>
    /// Testes unitários para a classe CreateSaidaAnimaisValidator
    /// </summary>
    public class CreateSaidaAnimaisValidatorTests
    {
        // Mock do contexto 
        private readonly Mock<ApplicationDbContext> _mockContext;

        // Instância do validador que será testado
        private readonly CreateSaidaAnimaisValidator _validator;

        // Mock do DbSet para simular a tabela de UnidadeExploracao
        private readonly Mock<DbSet<UnidadeExploracao>> _mockDbSet;

        /// <summary>
        /// Configura o ambiente de teste
        /// </summary>
        public CreateSaidaAnimaisValidatorTests()
        {
            // Inicializa os objetos mock
            _mockContext = new Mock<ApplicationDbContext>();
            _mockDbSet = new Mock<DbSet<UnidadeExploracao>>();

            // Cria dados de exemplo para unidades de exploração
            var unidades = new List<UnidadeExploracao>
            {
                new UnidadeExploracao { Id = 1, QuantidadeAnimais = 100 }, // Unidade com ID 1 e 100 animais
                new UnidadeExploracao { Id = 2, QuantidadeAnimais = 50 }    // Unidade com ID 2 e 50 animais
            }.AsQueryable(); // Converte para IQueryable para simular consulta 

            // Configura suporte a operações assíncronas no mock do DbSet
            _mockDbSet.As<IAsyncEnumerable<UnidadeExploracao>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<UnidadeExploracao>(unidades.GetEnumerator()));

            // Configura o provider para consultas assíncronas
            _mockDbSet.As<IQueryable<UnidadeExploracao>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<UnidadeExploracao>(unidades.Provider));

            // Configura propriedades básicas do IQueryable
            _mockDbSet.As<IQueryable<UnidadeExploracao>>().Setup(m => m.Expression).Returns(unidades.Expression);
            _mockDbSet.As<IQueryable<UnidadeExploracao>>().Setup(m => m.ElementType).Returns(unidades.ElementType);
            _mockDbSet.As<IQueryable<UnidadeExploracao>>().Setup(m => m.GetEnumerator()).Returns(() => unidades.GetEnumerator());

            // Configura o contexto mockado para retornar o DbSet mockado
            _mockContext.Setup(c => c.UnidadesExploracao).Returns(_mockDbSet.Object);

            // Cria instância do validador com o contexto mockado
            _validator = new CreateSaidaAnimaisValidator(_mockContext.Object);
        }

        /// <summary>
        /// Testa se o validador retorna erro quando a data de saída é futura
        /// </summary>
        [Fact]
        public async Task Should_HaveError_WhenDataSaidaIsFuture()
        {
            // Arrange 
            var dto = new CreateSaidaAnimaisDTO
            {
                DataSaida = DateTime.Now.AddDays(1), // Data futura (amanhã)
                CodigoUepOrigem = 1,                // Código de origem válido
                CodigoUepDestino = 2,               // Código de destino válido
                QuantidadeAnimais = 10              // Quantidade válida
            };

            // Act 
            var result = await _validator.ValidateAsync(dto);

            // Assert 
            Assert.False(result.IsValid); // Resultado não deve ser válido
            Assert.Contains(result.Errors, e => e.ErrorMessage == "A data de saída não pode ser futura."); // Deve conter o erro esperado
        }

        /// <summary>
        /// Testa se o validador retorna erro quando origem e destino são iguais
        /// </summary>
        [Fact]
        public async Task Should_HaveError_WhenOrigemEqualsDestino()
        {
            // Arrange
            var dto = new CreateSaidaAnimaisDTO
            {
                CodigoUepOrigem = 1,       // Origem
                CodigoUepDestino = 1,      // Destino igual à origem
                QuantidadeAnimais = 10,    // Quantidade válida
                DataSaida = DateTime.Now   // Data atual
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            Assert.False(result.IsValid); // Resultado não deve ser válido
            Assert.Contains(result.Errors, e => e.ErrorMessage == "A UEP de origem não pode ser igual à UEP de destino."); // Deve conter o erro esperado
        }

        /// <summary>
        /// Testa se o validador aceita dados válidos
        /// </summary>
        [Fact]
        public async Task Should_NotHaveError_WhenValid()
        {
            // Arrange
            var unidades = new List<UnidadeExploracao>
            {
                new UnidadeExploracao { Id = 1, QuantidadeAnimais = 100 }, // Quantidade suficiente
                new UnidadeExploracao { Id = 2, QuantidadeAnimais = 50 }
            }.AsQueryable();

            // Configura o mock para retornar a unidade correta
            _mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => unidades.FirstOrDefault(u => u.Id == (int)ids[0]));

            var dto = new CreateSaidaAnimaisDTO
            {
                CodigoUepOrigem = 1,                
                CodigoUepDestino = 2,               
                DataSaida = DateTime.Now.AddDays(-1),
                QuantidadeAnimais = 50              
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            Assert.True(result.IsValid); // Resultado deve ser válido
        }
    }

    /// <summary>
    /// Classe auxiliar para suporte a testes assíncronos
    /// </summary>
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner; 

        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

        /// Obtém o elemento atual
        public T Current => _inner.Current;

        /// Simula disposição assíncrona (não é necessária operação real)
        public ValueTask DisposeAsync() => default;

        /// Simula operação assíncrona de mover para o próximo elemento
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
    }

    /// <summary>
    /// Classe auxiliar para suporte a consultas assíncronas em testes
    /// </summary>
    internal class TestAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner; // Provider síncrono interno

        public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) => _inner.Execute(expression)!;
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

        /// <summary>
        /// Simula execução assíncrona de consulta
        /// Encapsula o resultado síncrono em uma tarefa completada
        /// </summary>
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default) =>
            (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(typeof(TResult).GetGenericArguments()[0])
                .Invoke(null, new[] { _inner.Execute(expression) })!;
    }


    /// <summary>
    /// Classe auxiliar para suporte a enumeração assíncrona em testes
    /// </summary>
    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
            new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}