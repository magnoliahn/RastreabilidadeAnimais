using FluentValidation;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.DTOs;

namespace rastreabilidadeAnimais.Validators
{
    /// <summary>
    /// Validador para o DTO de criação das UEP
    /// </summary>
    public class CreateUnidadeExploracaoValidator : AbstractValidator<CreateUnidadeExploracaoDTO>
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto 
        /// </summary>
        public CreateUnidadeExploracaoValidator(ApplicationDbContext context)
        {
            _context = context;

            // Regra para código da espécie
            RuleFor(x => x.CodigoEspecie)
                .NotEmpty()
                .WithMessage("O código da espécie é obrigatório.")
                .MustAsync(async (codigoEspecie, cancellation) => {
                    var especie = await _context.Especies.FindAsync(codigoEspecie);
                    return especie != null;
                })
                .WithMessage("A espécie informada não existe.");

            // Regra para quantidade de animais
            RuleFor(x => x.QuantidadeAnimais)
                .NotEmpty()
                .WithMessage("A quantidade de animais é obrigatória.")
                .GreaterThan(0)
                .WithMessage("A quantidade de animais deve ser maior que zero.");

            // Regra para código da propriedade
            RuleFor(x => x.CodigoPropriedade)
                .NotEmpty()
                .WithMessage("O código da propriedade é obrigatório.");
        }
    }

    /// <summary>
    /// Validador para o DTO de atualização das UEP
    /// </summary>
    public class UpdateUnidadeExploracaoValidator : AbstractValidator<UpdateUnidadeExploracaoDTO>
    {
        /// <summary>
        /// Construtor com regras de validação para atualização
        /// </summary>
        public UpdateUnidadeExploracaoValidator()
        {
            // Regra para quantidade de animais
            RuleFor(x => x.QuantidadeAnimais)
                .NotEmpty()
                .WithMessage("A quantidade de animais é obrigatória.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("A quantidade de animais não pode ser negativa.");
        }
    }
}