using FluentValidation;
using OrderService.Application.Commands;

namespace OrderService.Application.Validators;

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
    }
}

public sealed class MarkOrderPaidValidator : AbstractValidator<MarkOrderPaidCommand>
{
    public MarkOrderPaidValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}

public sealed class MarkOrderFailedValidator : AbstractValidator<MarkOrderFailedCommand>
{
    public MarkOrderFailedValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
    }
}
