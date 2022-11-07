﻿using System.Collections.Immutable;
using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public record Cart : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; } = default;
    public Customer? Customer { get; init; }
    public ImmutableList<CartItem> Items { get; init; } = ImmutableList<CartItem>.Empty;
    public DateTimeOffset LastAccess { get; init; } = DateTimeOffset.UtcNow;

    public string ConcurrencyToken { get; init; } = string.Empty;
}