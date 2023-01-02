import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { CheckoutComponent } from './components/checkout/checkout.component';
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
import { HomePageComponent } from './pages/home-page/home-page.component';
import {ReactiveFormsModule} from "@angular/forms";

@NgModule({
  declarations: [
    AppComponent,
    CheckoutComponent,
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
    HomePageComponent
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
