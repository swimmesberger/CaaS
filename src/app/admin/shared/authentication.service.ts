import { Injectable } from '@angular/core';
import {AuthConfig, OAuthService} from 'angular-oauth2-oidc';
import {authConfig} from "../auth.config";

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  constructor(private oauthService: OAuthService) {  }

  loadConfig(config: AuthConfig): Promise<boolean> {
    this.oauthService.configure(authConfig);
    // noinspection JSIgnoredPromiseFromCall
    return this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }

  login(): void {
    this.oauthService.initCodeFlow();
  }

  isLoggedIn() {
    return this.oauthService.hasValidAccessToken() &&
          this.oauthService.hasValidIdToken();
  }

  public get name() : string | null {
    let claims = this.oauthService.getIdentityClaims();
    if (!claims) return null;
    return claims['given_name'];
  }
}
