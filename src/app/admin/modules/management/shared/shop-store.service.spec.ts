import { TestBed } from '@angular/core/testing';

import { ShopStoreService } from './shop-store.service';

describe('ShopStoreService', () => {
  let service: ShopStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ShopStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
