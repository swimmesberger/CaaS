import { TestBed } from '@angular/core/testing';

import { AppKeyAuthenticationService } from './app-key-authentication.service';

describe('AuthenticationService', () => {
  let service: AppKeyAuthenticationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AppKeyAuthenticationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
