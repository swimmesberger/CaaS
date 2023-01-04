import { NgModule } from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {CanNavigateToAdminGuard} from "./can-navigate-to-admin.guard";
import {LoginComponent} from "./components/login/login.component";
import {DefaultLayoutComponent} from "./layouts/default-layout";
import {AdminComponent} from "./admin.component";

const routes: Routes = [
  { path: '', component: AdminComponent, canActivate: [CanNavigateToAdminGuard], children: [
      { path: '', component: DefaultLayoutComponent, canActivate: [CanNavigateToAdminGuard], children: [
        {
          path: 'dashboard',
          loadChildren: () =>
            import('./components/dashboard/dashboard.module').then((m) => m.DashboardModule)
        },
      ]}
  ]},
  { path: 'login', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
