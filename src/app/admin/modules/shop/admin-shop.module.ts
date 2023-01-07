import { NgModule } from '@angular/core';
import { AdminShopComponent } from './admin-shop.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {
    AlertModule,
    ButtonModule,
    CardModule,
    FormModule, GridModule, TableModule,
} from "@coreui/angular";
import { AppKeyLoginComponent } from './components/app-key-login/app-key-login.component';
import {AuthenticationApi} from "../../shared/authentication.api";
import {AppKeyAuthenticationService} from "./shared/app-key-authentication.service";
import {CaasAdminModule} from "../../caas-admin.module";
import {IconModule} from "@coreui/icons-angular";
import {NavProviderApi} from "../../shared/nav-provider.api";
import {ShopNavProviderService} from "./shared/shop-nav-provider.service";
import {AdminShopRoutingModule} from "./admin-shop-routing.module";
import {AsyncPipe, NgClass, NgForOf, NgIf} from "@angular/common";
import { DiscountsComponent } from './components/discounts/discounts.component';
import { DiscountEditComponent } from './components/discount-edit/discount-edit.component';


@NgModule({
  declarations: [
    AdminShopComponent,
    AppKeyLoginComponent,
    DiscountsComponent,
    DiscountEditComponent
  ],
  imports: [
    CaasAdminModule,
    AdminShopRoutingModule,
    ReactiveFormsModule,
    FormModule,
    CardModule,
    GridModule,
    ButtonModule,
    IconModule,
    FormsModule,
    NgIf,
    AlertModule,
    NgClass,
    TableModule,
    AsyncPipe,
    NgForOf
  ],
  providers: [
    { provide: AuthenticationApi, useClass: AppKeyAuthenticationService },
    { provide: NavProviderApi, useClass: ShopNavProviderService }
  ]
})
export class AdminShopModule { }
