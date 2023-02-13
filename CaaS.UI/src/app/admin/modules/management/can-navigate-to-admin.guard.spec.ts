import { TestBed } from '@angular/core/testing';

import { CanNavigateToAdminGuard } from './can-navigate-to-admin.guard';
import {OAuthModule} from "angular-oauth2-oidc";
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('ManagementCanNavigateToAdminGuard', () => {
  let guard: CanNavigateToAdminGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ HttpClientTestingModule, OAuthModule.forRoot() ]
    });
    guard = TestBed.inject(CanNavigateToAdminGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
