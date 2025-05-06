using FluentValidation;
using Microsoft.EntityFrameworkCore;
using rastreabilidadeAnimais.Data;
using rastreabilidadeAnimais.DTOs;

namespace rastreabilidadeAnimais.Validators
{
    /// <summary>
    /// Validador para o DTO de criação de saída de animais
    /// </summary>
    public class CreateSaidaAnimaisValidator : AbstractValidator<CreateSaidaAnimaisDTO>
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto 
        /// </summary>
        public CreateSaidaAnimaisValidator(ApplicationDbContext context)
        {
            _context = context;

            // Regra para data de saída
            RuleFor(x => x.DataSaida)
                .NotEmpty()
                .WithMessage("A data de saída é obrigatória.")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("A data de saída não pode ser futura.");

            // Regras para UEP de origem
            RuleFor(x => x.CodigoUepOrigem)
                .NotEmpty()
                .WithMessage("O código da UEP de origem é obrigatório.")
                .MustAsync(async (codigoUep, cancellation) =>
                    await _context.UnidadesExploracao.AnyAsync(u => u.Id == codigoUep))
                .WithMessage("A UEP de origem informada não existe.");

            // Regras para UEP de destino
            RuleFor(x => x.CodigoUepDestino)
                .NotEmpty()
                .WithMessage("O código da UEP de destino é obrigatório.")
                .MustAsync(async (codigoUep, cancellation) =>
                    await _context.UnidadesExploracao.AnyAsync(u => u.Id == codigoUep))
                .WithMessage("A UEP de destino informada não existe.")
                .NotEqual(x => x.CodigoUepOrigem)
                .WithMessage("A UEP de origem não pode ser igual à UEP de destino.");

            // Regra para quantidade de animais
            RuleFor(x => x.QuantidadeAnimais)
                .NotEmpty()
                .WithMessage("A quantidade de animais é obrigatória.")
                .GreaterThan(0)
                .WithMessage("A quantidade de animais deve ser maior que zero.");

            // Regra complexa para validar estoque na UEP de origem
            RuleFor(x => x)
                .MustAsync(async (x, cancellation) =>
                {
                    var uepOrigem = await _context.UnidadesExploracao
                        .Include(u => u.Especie)
                        .FirstOrDefaultAsync(u => u.Id == x.CodigoUepOrigem);
                    return uepOrigem != null && uepOrigem.QuantidadeAnimais >= x.QuantidadeAnimais;
                })
                .WithMessage("A quantidade de animais a serem movimentados excede a quantidade disponível na UEP de origem.")
                .MustAsync(async (x, cancellation) =>
                {
                    var origem = await _context.UnidadesExploracao.FindAsync(x.CodigoUepOrigem);
                    return origem?.QuantidadeAnimais >= x.QuantidadeAnimais;
                })
                .WithMessage("Quantidade excede o estoque da UEP de origem");
        }
    }

    /// <summary>
    /// Validador para o DTO de atualização de saída de animais
    /// </summary>
    public class UpdateSaidaAnimaisValidator : AbstractValidator<UpdateSaidaAnimaisDTO>
    {
        /// <summary>
        /// Construtor com regras de validação
        /// </summary>
        public UpdateSaidaAnimaisValidator()
        {
            // Regra para data de saída
            RuleFor(x => x.DataSaida)
                .NotEmpty()
                .WithMessage("A data de saída é obrigatória.")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("A data de saída não pode ser futura.");

            // Regra para quantidade de animais
            RuleFor(x => x.QuantidadeAnimais)
                .NotEmpty()
                .WithMessage("A quantidade de animais é obrigatória.")
                .GreaterThan(0)
                .WithMessage("A quantidade de animais deve ser maior que zero.");
        }
    }
}