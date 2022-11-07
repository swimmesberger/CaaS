﻿using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities;

public record OrderItem : IEntityBase {
    public Guid Id { get; init;  } = Guid.NewGuid();
    
    public Product Product { get; init; } = null!;
    public int Amount { get; init; }
    public decimal PricePerPiece { get; init; }
    public decimal DiscountPerPiece { get; init; } = 0;
    public decimal DiscountedPricePerPiece { get; init; } = 0;
    public decimal SumPerPosition { get; init; } = 0;
    public decimal DiscountedSumPerPosition { get; init; } = 0;
    public string ConcurrencyToken { get; init; } = string.Empty;
}