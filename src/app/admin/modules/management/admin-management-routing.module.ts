import { NgModule } from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {CanNavigateToAdminGuard} from "./can-navigate-to-admin.guard";
import {OAuthLoginComponent} from "./components/oauth-login/o-auth-login.component";
import {DefaultLayoutComponent} from "../../layouts/default-layout";
import {AdminManagementComponent} from "./admin-management.component";
import {ShopsComponent} from "./components/shops/shops.component";
import {ShopEditComponent} from "./components/shop-edit/shop-edit.component";

const routes: Routes = [
  { path: '', component: AdminManagementComponent, canActivate: [CanNavigateToAdminGuard], children: [
      { path: '', component: DefaultLayoutComponent, children: [
          {
            path: '',
            redirectTo: 'shops',
            pathMatch: 'full'
          },
          {
            path: 'shops',
            title: $localize `:@@adminShopsTitle:Shops`,
            data: {
              title: $localize `:@@adminShopsTitle:Shops`
            },
            component: ShopsComponent
          },
          {
            path: 'shop/:shopId',
            title: $localize `:@@adminShopEditTitle:Edit Shop`,
            data: {
              title: $localize `:@@adminShopEditTitle:Edit Shop`
            },
            component: ShopEditComponent
          },
          {
            path: 'shop',
            title: $localize `:@@adminShopAddTitle:Add Shop`,
            data: {
              title: $localize `:@@adminShopAddTitle:Add Shop`
            },
            component: ShopEditComponent
          },
      ]}
  ]},
  { path: 'login', component: OAuthLoginComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminManagementRoutingModule { }
