import { TestBed } from '@angular/core/testing';

import { OAuthAuthenticationService } from './o-auth-authentication.service';
import {OAuthModule} from "angular-oauth2-oidc";
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('OAuthAuthenticationService', () => {
  let service: OAuthAuthenticationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ HttpClientTestingModule, OAuthModule.forRoot() ]
    });
    service = TestBed.inject(OAuthAuthenticationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
