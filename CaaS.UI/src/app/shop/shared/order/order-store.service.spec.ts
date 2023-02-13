import { TestBed } from '@angular/core/testing';

import { OrderStoreService } from './order-store.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('OrderStoreService', () => {
  let service: OrderStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(OrderStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
