import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import {HttpClientModule} from "@angular/common/http";
import {PageNotFoundComponent} from "./components/page-not-found/page-not-found.component";
import {OAuthModule} from "angular-oauth2-oidc";
import {CaasCommonModule} from "./caas-common.module";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    CaasCommonModule,
    OAuthModule.forRoot()
  ],
  providers: [],
  exports: [],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor() {  }
}
