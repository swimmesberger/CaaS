import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { CartComponent } from './components/cart/cart.component';
import { ProductsComponent } from './components/products/products.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { CartPageComponent } from './pages/cart/cart-page.component';
import {ProductsPageComponent} from "./pages/products/products-page.component";
import { ProductDetailsComponent } from './components/product-details/product-details.component';
import { ProductPageComponent } from './pages/product/product-page.component';
import { ProductItemComponent } from './components/product-item/product-item.component';
import {HttpClientModule} from "@angular/common/http";
import { RatingBarComponent } from './components/rating-bar/rating-bar.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { WishlistButtonComponent } from './components/wishlist-button/wishlist-button.component';
import { ProductGalleryComponent } from './components/product-gallery/product-gallery.component';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { HomePageComponent } from './pages/home/home-page.component';
import {ReactiveFormsModule} from "@angular/forms";
import { CheckoutComponent } from './components/checkout/checkout.component';
import { CheckoutPageComponent } from './pages/checkout/checkout-page.component';
import { PromoInputComponent } from './components/promo-input/promo-input.component';
import { PaymentComponent } from './components/payment/payment.component';
import { PaymentPageComponent } from './pages/payment/payment-page.component';
import { CheckoutStepsComponent } from './components/checkout-steps/checkout-steps.component';
import { CheckoutSidebarComponent } from './components/checkout-sidebar/checkout-sidebar.component';
import { CreditcardComponent } from './components/creditcard/creditcard.component';
import { CheckoutReviewPageComponent } from './pages/checkout-review-page/checkout-review-page.component';
import { CheckoutReviewComponent } from './components/checkout-review/checkout-review.component';
import { OrderCompletePageComponent } from './pages/order-complete-page/order-complete-page.component';
import { ProductSearchComponent } from './components/product-search/product-search.component';
import { CartWidgetComponent } from './components/cart-widget/cart-widget.component';

@NgModule({
  declarations: [
    AppComponent,
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
    PageNotFoundComponent,
    WishlistButtonComponent,
    ProductGalleryComponent,
    BreadcrumbComponent,
    HomePageComponent,
    CheckoutComponent,
    CheckoutPageComponent,
    PromoInputComponent,
    PaymentComponent,
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
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor() {  }
}
