using FluentValidation;
using PaymentService.Application.Commands;

namespace PaymentService.Application.Validators;

public sealed class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public sealed class MarkPaymentCompletedValidator : AbstractValidator<MarkPaymentCompletedCommand>
{
    public MarkPaymentCompletedValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.TransactionId).NotEmpty();
    }
}

public sealed class MarkPaymentFailedValidator : AbstractValidator<MarkPaymentFailedCommand>
{
    public MarkPaymentFailedValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
    }
}
