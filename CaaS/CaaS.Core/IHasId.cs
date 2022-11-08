﻿namespace CaaS.Core; 

public interface IHasId {
    // guid to ensure we have a artificial primary key for each entity
    Guid Id { get; }
}