import { TestBed } from '@angular/core/testing';

import { TenantIdService } from './tenant-id.service';

describe('TenantIdService', () => {
  let service: TenantIdService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TenantIdService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
