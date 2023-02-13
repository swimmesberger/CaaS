import { TestBed } from '@angular/core/testing';

import { ManagementNavProviderService } from './management-nav-provider.service';

describe('ManagementNavProviderService', () => {
  let service: ManagementNavProviderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManagementNavProviderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
