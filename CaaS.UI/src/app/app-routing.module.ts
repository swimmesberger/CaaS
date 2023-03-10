import { NgModule } from '@angular/core';
import {RouterModule, Routes } from "@angular/router";
import {PageNotFoundComponent} from "./components/page-not-found/page-not-found.component";
import {HomeComponent} from "./components/home/home.component";

const routes: Routes = [
  { path: 'shop', loadChildren: () => import('./shop/shop.module').then(m => m.ShopModule) },
  { path: 'admin', loadChildren: () => import('./admin/modules/management/admin-management.module').then(m => m.AdminManagementModule) },
  { path: 'home', loadComponent: () => HomeComponent },
  { path: '',   redirectTo: '/home', pathMatch: 'full' },
  { path: '**', loadComponent: () => PageNotFoundComponent },  // Wildcard route for a 404 page
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
