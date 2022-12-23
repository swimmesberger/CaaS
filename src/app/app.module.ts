import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { FontAwesomeModule, FaIconLibrary } from '@fortawesome/angular-fontawesome';
import { faUser, faShoppingCart, faSearch } from "@fortawesome/free-solid-svg-icons";
import { CheckoutComponent } from './components/checkout/checkout.component';
import { ProductsComponent } from './components/products/products.component';
import { HeaderComponent } from './components/header/header.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { CartPageComponent } from './pages/cart/cart-page.component';
import {ProductsPageComponent} from "./pages/products/products-page.component";
import { ProductDetailsComponent } from './components/product-details/product-details.component';
import { ProductPageComponent } from './pages/product/product-page.component';
import { ProductItemComponent } from './components/product-item/product-item.component';

@NgModule({
  declarations: [
    AppComponent,
    CheckoutComponent,
    ProductsComponent,
    HeaderComponent,
    PageHeaderComponent,
    ProductsPageComponent,
    CartPageComponent,
    ProductDetailsComponent,
    ProductPageComponent,
    ProductItemComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FontAwesomeModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(library: FaIconLibrary) {
    library.addIcons(faUser, faShoppingCart, faSearch);
  }
}
