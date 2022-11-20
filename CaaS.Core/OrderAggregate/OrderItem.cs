﻿using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.ProductAggregate;

namespace CaaS.Core.OrderAggregate;

public record OrderItem : IEntityBase {
    public Guid Id { get; init;  } = Guid.NewGuid();
    
    public Product Product { get; init; } = null!;
    public Guid ShopId { get; init; }
    public Guid OrderId { get; init; }
    public int Amount { get; init; }
    
    public IImmutableList<ItemDiscount> OrderItemDiscounts { get; init; } = ImmutableArray<ItemDiscount>.Empty;
    public decimal PricePerPiece { get; init; }
    public string ConcurrencyToken { get; init; } = string.Empty;
}