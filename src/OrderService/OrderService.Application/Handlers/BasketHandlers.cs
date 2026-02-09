using MediatR;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Domain;

namespace OrderService.Application.Handlers;

public sealed class AddBasketItemHandler : IRequestHandler<AddBasketItemCommand, BasketDto>
{
    private readonly IBasketRepository _baskets;
    public AddBasketItemHandler(IBasketRepository baskets) => _baskets = baskets;

    public async Task<BasketDto> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basket = await _baskets.GetByUserIdAsync(request.UserId) ?? Basket.Create(Guid.NewGuid(), request.UserId);
        basket.AddItem(request.Item.ProductId, request.Item.ProductName, request.Item.UnitPrice, request.Item.Quantity);
        await _baskets.UpsertAsync(basket);
        await _baskets.SaveChangesAsync();
        return Map(basket);
    }

    private static BasketDto Map(Basket b) => new(b.Id, b.UserId, b.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());
}

public sealed class RemoveBasketItemHandler : IRequestHandler<RemoveBasketItemCommand, BasketDto>
{
    private readonly IBasketRepository _baskets;
    public RemoveBasketItemHandler(IBasketRepository baskets) => _baskets = baskets;

    public async Task<BasketDto> Handle(RemoveBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basket = await _baskets.GetByUserIdAsync(request.UserId) ?? Basket.Create(Guid.NewGuid(), request.UserId);
        basket.RemoveItem(request.ProductId);
        await _baskets.UpsertAsync(basket);
        await _baskets.SaveChangesAsync();
        return new BasketDto(basket.Id, basket.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());
    }
}

public sealed class ClearBasketHandler : IRequestHandler<ClearBasketCommand, BasketDto>
{
    private readonly IBasketRepository _baskets;
    public ClearBasketHandler(IBasketRepository baskets) => _baskets = baskets;

    public async Task<BasketDto> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _baskets.GetByUserIdAsync(request.UserId) ?? Basket.Create(Guid.NewGuid(), request.UserId);
        basket.Clear();
        await _baskets.UpsertAsync(basket);
        await _baskets.SaveChangesAsync();
        return new BasketDto(basket.Id, basket.UserId, Array.Empty<BasketItemDto>());
    }
}

public sealed class GetBasketHandler : IRequestHandler<GetBasketQuery, BasketDto?>
{
    private readonly IBasketRepository _baskets;
    public GetBasketHandler(IBasketRepository baskets) => _baskets = baskets;

    public async Task<BasketDto?> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await _baskets.GetByUserIdAsync(request.UserId);
        return basket == null ? null : new BasketDto(basket.Id, basket.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());
    }
}
