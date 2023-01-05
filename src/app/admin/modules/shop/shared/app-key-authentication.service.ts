import { Injectable } from '@angular/core';
import {ShopAdminDto} from "../../../shared/models/shopAdminDto";
import {Md5} from "ts-md5";
import {AuthenticationApi} from "../../../shared/authentication.api";

@Injectable({
  providedIn: 'root'
})
export class AppKeyAuthenticationService implements AuthenticationApi {
  private _appKey: string | null = null;
  private _shopAdmin: ShopAdminDto | null = null;

  constructor() {  }

  async login(email: string, appKey: string): Promise<boolean> {
    this._appKey = appKey;
    return true;
  }

  isLoggedIn() {
    return this._appKey != null;
  }

  public get name(): string | null {
    return this._shopAdmin?.name ?? null;
  }

  public get eMail(): string | null {
    return this._shopAdmin?.email ?? null;
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
