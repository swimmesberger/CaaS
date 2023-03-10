import { NgModule } from '@angular/core';
import {Route, RouterModule, Routes} from "@angular/router";
import {ShopComponent} from "./shop.component";
import {CheckoutCompleteComponent} from "./components/checkout-complete/checkout-complete.component";
import {PageHeaderLayoutComponent} from "./layouts/page-header-layout/page-header-layout.component";
import {ProductsComponent} from "./components/products/products.component";
import {ProductDetailsComponent} from "./components/product-details/product-details.component";
import {CartComponent} from "./components/cart/cart.component";
import {CheckoutComponent} from "./components/checkout/checkout.component";
import {CheckoutPaymentComponent} from "./components/checkout-payment/checkout-payment.component";
import {CheckoutReviewComponent} from "./components/checkout-review/checkout-review.component";
import {PageNavbarLayoutComponent} from "./layouts/page-navbar-layout/page-navbar-layout.component";
import {PageSimpleLayoutComponent} from "./layouts/page-simple-layout/page-simple-layout.component";

function createPageHeaderRoute(route: Route) {
  const path = route.path;
  route.path = '';
  return { path: path, component: PageHeaderLayoutComponent, children: [route] }
}

function createPageNavbarRoute(route: Route) {
  const path = route.path;
  route.path = '';
  return { path: path, component: PageNavbarLayoutComponent, children: [route] }
}

const routes: Routes = [
  { path: '', component: ShopComponent, children: [
      { path: ':shopId', component: PageSimpleLayoutComponent, data: { tenantUser: true }, children: [
        {
          path: '',
          redirectTo: 'products',
          pathMatch: 'full'
        },
        createPageHeaderRoute({
          path: 'products',
          component: ProductsComponent,
          title: $localize `:@@productPageTitle:Products`
        }),
        createPageHeaderRoute({
          path: 'product/:id',
          component: ProductDetailsComponent,
          title: $localize `:@@productDetailsPageTitle:Product Details`
        }),
        createPageHeaderRoute({
          path: 'cart',
          component: CartComponent,
          title: $localize `:@@cartPageTitle:Your cart`
        }),
        createPageHeaderRoute({
          path: 'checkout',
          component: CheckoutComponent,
          title: $localize `:@@checkoutPageTitle:Checkout`
        }),
        createPageHeaderRoute({
          path: 'checkout/payment',
          component: CheckoutPaymentComponent,
          title: $localize `:@@checkoutPageTitle:Checkout`
        }),
        createPageHeaderRoute({
          path: 'checkout/review',
          component: CheckoutReviewComponent,
          title: $localize `:@@checkoutPageTitle:Checkout`
        }),
        createPageNavbarRoute({
          path: 'checkout/complete',
          component: CheckoutCompleteComponent,
          title: $localize `:@@checkoutCompletePageTitle:Checkout Complete`
        })
      ]}
    ]
  },
  {
    path: ':shopId/admin',
    data: { tenantUser: true },
    loadChildren: () => import('../admin/modules/shop/admin-shop.module').then(m => m.AdminShopModule)
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ShopRoutingModule { }
