import { NgModule } from '@angular/core';
import {CartPageComponent} from "./pages/cart/cart-page.component";
import {OrderCompletePageComponent} from "./pages/order-complete-page/order-complete-page.component";
import {RouterModule, Routes} from "@angular/router";
import {PaymentPageComponent} from "./pages/payment/payment-page.component";
import {CheckoutPageComponent} from "./pages/checkout/checkout-page.component";
import {CheckoutReviewPageComponent} from "./pages/checkout-review-page/checkout-review-page.component";
import {HomePageComponent} from "./pages/home/home-page.component";
import {ProductPageComponent} from "./pages/product/product-page.component";
import {ProductsPageComponent} from "./pages/products/products-page.component";
import {ShopComponent} from "./shop.component";

const routes: Routes = [
  {
    path: '', component: ShopComponent, children: [
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
        component: CheckoutReviewPageComponent,
        title: $localize `:@@checkoutPageTitle:Checkout`
      },
      {
        path: 'checkout/complete',
        component: OrderCompletePageComponent,
        title: $localize `:@@checkoutCompletePageTitle:Checkout Complete`
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ShopRoutingModule { }
