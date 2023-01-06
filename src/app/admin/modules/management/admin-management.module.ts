import { NgModule } from '@angular/core';
import { AdminManagementComponent } from './admin-management.component';
import {ReactiveFormsModule} from "@angular/forms";
import {AdminManagementRoutingModule} from "./admin-management-routing.module";
import { OAuthLoginComponent } from './components/oauth-login/o-auth-login.component';
import {AuthenticationApi} from "../../shared/authentication.api";
import {OAuthAuthenticationService} from "./shared/o-auth-authentication.service";
import {CaasAdminModule} from "../../caas-admin.module";
import {CardModule, FormModule, GridModule, TableModule} from "@coreui/angular";
import {NavProviderApi} from "../../shared/nav-provider.api";
import {ManagementNavProviderService} from "./shared/management-nav-provider.service";
import { ShopsComponent } from './components/shops/shops.component';
import {IconModule} from "@coreui/icons-angular";
import {AsyncPipe, NgForOf} from "@angular/common";
import { ShopEditComponent } from './components/shop-edit/shop-edit.component';

@NgModule({
  declarations: [
    AdminManagementComponent,
    OAuthLoginComponent,
    ShopsComponent,
    ShopEditComponent
  ],
  imports: [
    CaasAdminModule,
    AdminManagementRoutingModule,
    ReactiveFormsModule,
    FormModule,
    CardModule,
    GridModule,
    TableModule,
    IconModule,
    NgForOf,
    AsyncPipe
  ],
  providers: [
    { provide: AuthenticationApi, useClass: OAuthAuthenticationService },
    { provide: NavProviderApi, useClass: ManagementNavProviderService }
  ]
})
export class AdminManagementModule { }
