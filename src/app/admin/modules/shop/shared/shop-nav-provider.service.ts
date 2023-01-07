import { Injectable } from '@angular/core';
import {NavProviderApi} from "../../../shared/nav-provider.api";
import {INavData} from "@coreui/angular";
import {TenantIdService} from "../../../../shared/tenant-id.service";

@Injectable({
  providedIn: 'root'
})
export class ShopNavProviderService implements NavProviderApi {
  private readonly _navItems: INavData[];

  constructor(tenantService: TenantIdService) {
    this._navItems = [
      {
        title: true,
        name: 'Management'
      },
      {
        name: 'Dashboard',
        url: `${tenantService.tenantUrl}/admin/dashboard`,
        iconComponent: { name: 'cil-speedometer' }
      },
      {
        name: 'Discount Rules',
        url: `${tenantService.tenantUrl}/admin/discounts`,
        iconComponent: { name: 'cil-money' }
      },
      {
        name: 'Add Discount Rule',
        url: `${tenantService.tenantUrl}/admin/discount`,
        iconComponent: { name: 'cil-plus' }
      }
    ];
  }

  navItems(): Promise<Array<INavData>> {
    return Promise.resolve(this._navItems);
  }
}
