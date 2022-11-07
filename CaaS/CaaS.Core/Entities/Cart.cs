﻿using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public class Cart : IEntityBase {
    public Guid Id { get; init; }
    public Shop Shop { get; init; } = null!;
    public Customer Customer { get; init; } = null!;
    public List<CartItem> Products { get; init; } = new();

    public string ConcurrencyToken { get; init; } = string.Empty;
}