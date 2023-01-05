import { NgModule } from '@angular/core';
import { AdminShopComponent } from './admin-shop.component';
import {ReactiveFormsModule} from "@angular/forms";
import {
  ButtonModule,
  CardModule,
  FormModule, GridModule,
} from "@coreui/angular";
import { AppKeyLoginComponent } from './components/app-key-login/app-key-login.component';
import {AuthenticationApi} from "../../shared/authentication.api";
import {AppKeyAuthenticationService} from "./shared/app-key-authentication.service";
import {CaasAdminModule} from "../../caas-admin.module";
import {IconModule} from "@coreui/icons-angular";
import {NavProviderApi} from "../../shared/nav-provider.api";
import {ShopNavProviderService} from "./shared/shop-nav-provider.service";
import {AdminShopRoutingModule} from "./admin-shop-routing.module";


@NgModule({
  declarations: [
    AdminShopComponent,
    AppKeyLoginComponent
  ],
  imports: [
    CaasAdminModule,
    AdminShopRoutingModule,
    ReactiveFormsModule,
    FormModule,
    CardModule,
    GridModule,
    ButtonModule,
    IconModule
  ],
  providers: [
    { provide: AuthenticationApi, useClass: AppKeyAuthenticationService },
    { provide: NavProviderApi, useClass: ShopNavProviderService }
  ]
})
export class AdminShopModule { }
