import { NgModule } from '@angular/core';
import {AdminComponent} from "./admin.component";
import {RouterModule, Routes} from "@angular/router";
import {CanNavigateToAdminGuard} from "./can-navigate-to-admin.guard";
import {LoginComponent} from "./components/login/login.component";

const routes: Routes = [
  {
    path: '', component: AdminComponent, canActivate: [CanNavigateToAdminGuard], children: [

    ]
  },
  { path: 'login', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
