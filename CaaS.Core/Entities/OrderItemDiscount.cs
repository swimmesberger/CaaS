﻿using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public class OrderItemDiscount : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid ShopId { get; init; } = default;
    public Guid OrderItemId { get; init; } = default;
    public string DiscountName { get; init; } = string.Empty;
    public decimal DiscountValue { get; init; } = 0;
    public string ConcurrencyToken { get; init; } = string.Empty;
}