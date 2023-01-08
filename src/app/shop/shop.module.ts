import {DEFAULT_CURRENCY_CODE, LOCALE_ID, NgModule} from '@angular/core';

import {ReactiveFormsModule} from "@angular/forms";
import {ShopRoutingModule} from "./shop-routing.module";
import {WishlistButtonComponent} from "./components/wishlist-button/wishlist-button.component";
import {NavbarComponent} from "./components/navbar/navbar.component";
import {CheckoutPaymentComponent} from "./components/checkout-payment/checkout-payment.component";
import {PromoInputComponent} from "./components/promo-input/promo-input.component";
import {CheckoutComponent} from "./components/checkout/checkout.component";
import {ProductGalleryComponent} from "./components/product-gallery/product-gallery.component";
import {CheckoutSidebarComponent} from "./components/checkout-sidebar/checkout-sidebar.component";
import {PageHeaderComponent} from "./components/page-header/page-header.component";
import {RatingBarComponent} from "./components/rating-bar/rating-bar.component";
import {CreditcardComponent} from "./components/creditcard/creditcard.component";
import {ProductsComponent} from "./components/products/products.component";
import {HomePageComponent} from "./components/home/home-page.component";
import {CheckoutStepsComponent} from "./components/checkout-steps/checkout-steps.component";
import {ProductSearchComponent} from "./components/product-search/product-search.component";
import {CheckoutReviewComponent} from "./components/checkout-review/checkout-review.component";
import {CartComponent} from "./components/cart/cart.component";
import {BreadcrumbComponent} from "./components/breadcrumb/breadcrumb.component";
import {ProductDetailsComponent} from "./components/product-details/product-details.component";
import {ProductItemComponent} from "./components/product-item/product-item.component";
import {CartWidgetComponent} from "./components/cart-widget/cart-widget.component";
import {ShopComponent} from "./shop.component";
import {CommonModule, getLocaleCurrencyCode} from "@angular/common";
import {CheckoutCompleteComponent} from "./components/checkout-complete/checkout-complete.component";
import { PageHeaderLayoutComponent } from './layouts/page-header-layout/page-header-layout.component';
import { PageNavbarLayoutComponent } from './layouts/page-navbar-layout/page-navbar-layout.component';
import { PageSimpleLayoutComponent } from './layouts/page-simple-layout/page-simple-layout.component';

@NgModule({
  declarations: [
    ShopComponent,
    CartComponent,
    ProductsComponent,
    NavbarComponent,
    PageHeaderComponent,
    ProductDetailsComponent,
    ProductItemComponent,
    RatingBarComponent,
    WishlistButtonComponent,
    ProductGalleryComponent,
    BreadcrumbComponent,
    HomePageComponent,
    CheckoutComponent,
    PromoInputComponent,
    CheckoutPaymentComponent,
    CheckoutStepsComponent,
    CheckoutSidebarComponent,
    CreditcardComponent,
    CheckoutReviewComponent,
    CheckoutCompleteComponent,
    ProductSearchComponent,
    CartWidgetComponent,
    PageHeaderLayoutComponent,
    PageNavbarLayoutComponent,
    PageSimpleLayoutComponent
  ],
  imports: [
    CommonModule,
    ShopRoutingModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: DEFAULT_CURRENCY_CODE, useFactory: getLocaleCurrencyCode, deps: [LOCALE_ID] },
  ]
})
export class ShopModule { }
