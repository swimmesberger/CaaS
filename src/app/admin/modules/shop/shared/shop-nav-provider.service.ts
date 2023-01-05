import { Injectable } from '@angular/core';
import {NavProviderApi} from "../../../shared/nav-provider.api";
import {INavData} from "@coreui/angular";
import {navItems} from "./_nav";

@Injectable({
  providedIn: 'root'
})
export class ShopNavProviderService implements NavProviderApi {
  constructor() { }

  navItems(): Promise<Array<INavData>> {
    return Promise.resolve(navItems);
  }
}
