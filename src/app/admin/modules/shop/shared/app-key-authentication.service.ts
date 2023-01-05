import { Injectable } from '@angular/core';
import {ShopAdminDto} from "../../../shared/models/shopAdminDto";
import {Md5} from "ts-md5";
import {AuthenticationApi} from "../../../shared/authentication.api";
import {lastValueFrom} from "rxjs";
import {ShopStoreService} from "./shop-store.service";
import {HttpErrorResponse} from "@angular/common/http";
import {TenantIdService} from "../../../../shared/tenant-id.service";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AppKeyAuthenticationService implements AuthenticationApi {
  private static readonly AppKeyKey = "appKeyAuth";
  private static readonly AppKeyShopAdmin = "shopAdmin";

  constructor(private _router: Router,
              private _shopService: ShopStoreService,
              private _tenantService: TenantIdService) {  }

  async login(email: string, appKey: string, redirectUrl: string | null = null): Promise<boolean> {
    try {
      const shop = await lastValueFrom(this._shopService.getShopByAdmin(email, appKey));
      localStorage.setItem(this.appKeyKey, appKey);
      if (shop.shopAdmin) {
        localStorage.setItem(this.shopAdminKey, JSON.stringify(shop.shopAdmin));
      }
      if (!redirectUrl) {
        redirectUrl = `${this._tenantService.tenantUrl}/admin`
      }
      // noinspection ES6MissingAwait
      this._router.navigateByUrl(redirectUrl);
      return true;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        if (error.status === 401 || error.status === 404) {
          return false;
        }
      }
      throw error;
    }
  }

  logout(): void {
    localStorage.removeItem(this.appKeyKey);
    localStorage.removeItem(this.shopAdminKey);
    // noinspection JSIgnoredPromiseFromCall
    this._router.navigateByUrl(`${this._tenantService.tenantUrl}/admin/login`)
  }

  isLoggedIn() {
    return this.appKey != null;
  }

  public get name(): string | null {
    return this.shopAdmin?.name ?? null;
  }

  public get eMail(): string | null {
    return this.shopAdmin?.eMail ?? null;
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

  private get appKeyKey(): string {
    return `${this._tenantService.tenantId}_${AppKeyAuthenticationService.AppKeyKey}`
  }

  private get shopAdminKey(): string {
    return `${this._tenantService.tenantId}_${AppKeyAuthenticationService.AppKeyShopAdmin}`
  }

  private get appKey(): string | null {
    return localStorage.getItem(this.appKeyKey);
  }

  private get shopAdmin(): ShopAdminDto | null {
    const json = localStorage.getItem(this.shopAdminKey);
    if(!json) return null;
    return JSON.parse(json);
  }
}
