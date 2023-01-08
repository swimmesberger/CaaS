import { TestBed } from '@angular/core/testing';

import { ShopStoreService } from './shop-store.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('ShopStoreService', () => {
  let service: ShopStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(ShopStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
