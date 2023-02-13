import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AppKeyAuthenticationService } from './shared/app-key-authentication.service';
import {TenantIdService} from "../../../shared/tenant-id.service";

@Injectable({
  providedIn: 'root'
})
export class CanNavigateToAdminGuard implements CanActivate {

  constructor(protected router: Router,
              protected auth: AppKeyAuthenticationService,
              private tenantServcie: TenantIdService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      const tenantId = this.tenantServcie.getTenantId(state.root, true);
      this.tenantServcie.setTenantId(tenantId);
      if (!this.auth.isLoggedIn()) {
        // noinspection JSIgnoredPromiseFromCall
        this.router.navigate([this.tenantServcie.tenantUrl + '/admin/login'], {
          queryParams: {
            redirectUri: state.url
          }
        });
        return false;
      } else {
        return true;
      }
  }
}
