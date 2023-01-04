import { Injectable } from '@angular/core';
import {AuthConfig, OAuthService} from 'angular-oauth2-oidc';
import {authConfig} from "../auth.config";
import {Md5} from 'ts-md5';

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

  public get name(): string | null {
    let claims = this.oauthService.getIdentityClaims();
    if (!claims) return null;
    return claims['given_name'];
  }

  public get eMail(): string | null {
    let claims = this.oauthService.getIdentityClaims();
    if (!claims) return null;
    return claims['email'];
  }

  public gravatarURL(size: number): string | null {
    const eMail = this.eMail;
    if (!eMail) return null;
    // Trim leading and trailing whitespace from
    // an email address and force all characters
    // to lower case
    const address = eMail.trim().toLowerCase();

    // Create an MD5 hash of the final string
    const hash = Md5.hashStr(address);

    // Grab the actual image URL
    return `https://www.gravatar.com/avatar/${ hash }?s=${size}`;
  }
}
