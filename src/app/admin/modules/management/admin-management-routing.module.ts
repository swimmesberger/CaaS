import { NgModule } from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {CanNavigateToAdminGuard} from "./can-navigate-to-admin.guard";
import {OAuthLoginComponent} from "./components/oauth-login/o-auth-login.component";
import {DefaultLayoutComponent} from "../../layouts/default-layout";
import {AdminManagementComponent} from "./admin-management.component";

const routes: Routes = [
  { path: '', component: AdminManagementComponent, canActivate: [CanNavigateToAdminGuard], children: [
      { path: '', component: DefaultLayoutComponent, children: [

      ]}
  ]},
  { path: 'login', component: OAuthLoginComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminManagementRoutingModule { }
