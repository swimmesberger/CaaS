import { NgModule } from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {DefaultLayoutComponent} from "../../layouts/default-layout";
import {AdminShopComponent} from "./admin-shop.component";
import {AppKeyLoginComponent} from "./components/app-key-login/app-key-login.component";
import {CanNavigateToAdminGuard} from "./can-navigate-to-admin.guard";

const routes: Routes = [
  { path: '', component: AdminShopComponent, children: [
      { path: '', component: DefaultLayoutComponent, canActivate: [CanNavigateToAdminGuard], children: [
        {
          path: 'dashboard',
          loadChildren: () =>
            import('./components/dashboard/dashboard.module').then((m) => m.DashboardModule)
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
