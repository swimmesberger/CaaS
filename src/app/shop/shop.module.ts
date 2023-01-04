import { NgModule } from '@angular/core';

import {ReactiveFormsModule} from "@angular/forms";
import {ShopRoutingModule} from "./shop-routing.module";
import {CheckoutPageComponent} from "./pages/checkout/checkout-page.component";
import {WishlistButtonComponent} from "./components/wishlist-button/wishlist-button.component";
import {NavbarComponent} from "./components/navbar/navbar.component";
import {ProductPageComponent} from "./pages/product/product-page.component";
import {CheckoutPaymentComponent} from "./components/checkout-payment/checkout-payment.component";
import {PromoInputComponent} from "./components/promo-input/promo-input.component";
import {CheckoutComponent} from "./components/checkout/checkout.component";
import {ProductGalleryComponent} from "./components/product-gallery/product-gallery.component";
import {PaymentPageComponent} from "./pages/payment/payment-page.component";
import {CheckoutSidebarComponent} from "./components/checkout-sidebar/checkout-sidebar.component";
import {PageHeaderComponent} from "./components/page-header/page-header.component";
import {RatingBarComponent} from "./components/rating-bar/rating-bar.component";
import {CreditcardComponent} from "./components/creditcard/creditcard.component";
import {ProductsComponent} from "./components/products/products.component";
import {HomePageComponent} from "./pages/home/home-page.component";
import {ProductsPageComponent} from "./pages/products/products-page.component";
import {CheckoutStepsComponent} from "./components/checkout-steps/checkout-steps.component";
import {CartPageComponent} from "./pages/cart/cart-page.component";
import {ProductSearchComponent} from "./components/product-search/product-search.component";
import {OrderCompletePageComponent} from "./pages/order-complete-page/order-complete-page.component";
import {CheckoutReviewComponent} from "./components/checkout-review/checkout-review.component";
import {CartComponent} from "./components/cart/cart.component";
import {BreadcrumbComponent} from "./components/breadcrumb/breadcrumb.component";
import {ProductDetailsComponent} from "./components/product-details/product-details.component";
import {ProductItemComponent} from "./components/product-item/product-item.component";
import {CartWidgetComponent} from "./components/cart-widget/cart-widget.component";
import {CheckoutReviewPageComponent} from "./pages/checkout-review-page/checkout-review-page.component";
import {HttpClientModule} from "@angular/common/http";
import {ShopComponent} from "./shop.component";
import {CommonModule} from "@angular/common";

@NgModule({
  declarations: [
    ShopComponent,
    CartComponent,
    ProductsComponent,
    NavbarComponent,
    PageHeaderComponent,
    ProductsPageComponent,
    CartPageComponent,
    ProductDetailsComponent,
    ProductPageComponent,
    ProductItemComponent,
    RatingBarComponent,
    WishlistButtonComponent,
    ProductGalleryComponent,
    BreadcrumbComponent,
    HomePageComponent,
    CheckoutComponent,
    CheckoutPageComponent,
    PromoInputComponent,
    CheckoutPaymentComponent,
    PaymentPageComponent,
    CheckoutStepsComponent,
    CheckoutSidebarComponent,
    CreditcardComponent,
    CheckoutReviewPageComponent,
    CheckoutReviewComponent,
    OrderCompletePageComponent,
    ProductSearchComponent,
    CartWidgetComponent
  ],
  imports: [
    CommonModule,
    ShopRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  providers: []
})
export class ShopModule { }
