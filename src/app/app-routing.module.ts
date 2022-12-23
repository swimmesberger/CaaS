import { NgModule } from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {ProductsPageComponent} from "./pages/products/products-page.component";
import {CartPageComponent} from "./pages/cart/cart-page.component";
import {ProductPageComponent} from "./pages/product/product-page.component";

const routes: Routes = [
  { path: 'product', component: ProductPageComponent },
  { path: 'products', component: ProductsPageComponent },
  { path: 'cart', component: CartPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
