﻿using CaaS.Core.Base;
using CaaS.Core.ShopAggregate;

namespace CaaS.Core.ProductAggregate; 

public record Product : IEntityBase {
    public Guid Id { get; init; }
    public Shop Shop { get; init; } = null!;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DownloadLink { get; init; } = string.Empty;
    public decimal Price { get; init; } = 0;

    public string ConcurrencyToken { get; init; } = string.Empty;
}