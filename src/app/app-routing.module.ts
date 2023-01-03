import { NgModule } from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {ProductsPageComponent} from "./pages/products/products-page.component";
import {CartPageComponent} from "./pages/cart/cart-page.component";
import {ProductPageComponent} from "./pages/product/product-page.component";
import {PageNotFoundComponent} from "./components/page-not-found/page-not-found.component";
import {HomePageComponent} from "./pages/home-page/home-page.component";
import {CheckoutPageComponent} from "./pages/checkout/checkout-page.component";
import {PaymentPageComponent} from "./pages/payment-page/payment-page.component";

const routes: Routes = [
  {
    path: '',
    component: HomePageComponent,
    title: $localize `:@@homePageTitle:Home`
  },
  {
    path: 'products',
    component: ProductsPageComponent,
    title: $localize `:@@productPageTitle:Products`
  },
  {
    path: 'product/:id',
    component: ProductPageComponent,
    title: $localize `:@@productDetailsPageTitle:Product Details`
  },
  {
    path: 'cart',
    component: CartPageComponent,
    title: $localize `:@@cartPageTitle:Your cart`
  },
  {
    path: 'checkout',
    component: CheckoutPageComponent,
    title: $localize `:@@checkoutPageTitle:Checkout`
  },
  {
    path: 'checkout/payment',
    component: PaymentPageComponent,
    title: $localize `:@@checkoutPageTitle:Checkout`
  },
  {
    path: 'checkout/review',
    component: PaymentPageComponent,
    title: $localize `:@@checkoutPageTitle:Checkout`
  },
  { path: '**', component: PageNotFoundComponent },  // Wildcard route for a 404 page
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
