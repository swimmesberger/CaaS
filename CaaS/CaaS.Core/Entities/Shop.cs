﻿using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record Shop : IEntityBase {
    public const int DefaultCartLifetimeMinutes = 120;

    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CartLifetimeMinutes { get; init; } = DefaultCartLifetimeMinutes;
    public ShopAdmin ShopAdmin { get; init; } = null!;
    public string ConcurrencyToken { get; init; } = string.Empty;
}