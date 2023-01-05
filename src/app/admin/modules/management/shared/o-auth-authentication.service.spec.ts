import { TestBed } from '@angular/core/testing';

import { OAuthAuthenticationService } from './o-auth-authentication.service';

describe('AuthenticationService', () => {
  let service: OAuthAuthenticationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OAuthAuthenticationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
