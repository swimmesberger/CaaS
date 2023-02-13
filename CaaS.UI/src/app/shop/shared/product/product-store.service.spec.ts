import { TestBed } from '@angular/core/testing';

import { ProductStoreService } from './product-store.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('ProductStoreService', () => {
  let service: ProductStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(ProductStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
