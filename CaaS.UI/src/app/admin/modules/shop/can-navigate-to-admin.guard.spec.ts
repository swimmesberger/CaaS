import { TestBed } from '@angular/core/testing';

import { CanNavigateToAdminGuard } from './can-navigate-to-admin.guard';
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('ShopCanNavigateToAdminGuard', () => {
  let guard: CanNavigateToAdminGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ HttpClientTestingModule ]
    });
    guard = TestBed.inject(CanNavigateToAdminGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
