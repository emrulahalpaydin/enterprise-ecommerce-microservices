using FluentValidation;
using OrderService.Application.Commands;

namespace OrderService.Application.Validators;

public sealed class AddBasketItemValidator : AbstractValidator<AddBasketItemCommand>
{
    public AddBasketItemValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Item.ProductId).NotEmpty();
        RuleFor(x => x.Item.Quantity).GreaterThan(0);
    }
}

public sealed class RemoveBasketItemValidator : AbstractValidator<RemoveBasketItemCommand>
{
    public RemoveBasketItemValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}

public sealed class ClearBasketValidator : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
