import { NgModule } from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {DefaultLayoutComponent} from "../../layouts/default-layout";
import {AdminShopComponent} from "./admin-shop.component";
import {AppKeyLoginComponent} from "./components/app-key-login/app-key-login.component";
import {CanNavigateToAdminGuard} from "./can-navigate-to-admin.guard";
import {DiscountsComponent} from "./components/discounts/discounts.component";
import {DiscountEditComponent} from "./components/discount-edit/discount-edit.component";

const routes: Routes = [
  { path: '', component: AdminShopComponent, children: [
      { path: '', component: DefaultLayoutComponent, canActivate: [CanNavigateToAdminGuard], children: [
        {
          path: '',
          redirectTo: 'dashboard',
          pathMatch: 'full'
        },
        {
          path: 'dashboard',
          loadChildren: () =>
            import('./components/dashboard/dashboard.module').then((m) => m.DashboardModule)
        },
        {
          path: 'discounts',
          data: {
            title: $localize `:@@shopAdminDiscountsTitle:Discounts`
          },
          component: DiscountsComponent
        },
        {
          path: 'discount/:discountId',
          data: {
            title: $localize `:@@shopAdminEditDiscountTitle:Edit Discount`
          },
          component: DiscountEditComponent
        },
        {
          path: 'discount',
          data: {
            title: $localize `:@@shopAdminAddDiscountTitle:Add Discount`
          },
          component: DiscountEditComponent
        },
      ]},
      { path: 'login', component: AppKeyLoginComponent }
  ]}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminShopRoutingModule { }
