import { TestBed } from '@angular/core/testing';

import { CartStoreService } from './cart-store.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('CartStoreService', () => {
  let service: CartStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(CartStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
