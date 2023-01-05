import { NgModule } from '@angular/core';
import { AdminManagementComponent } from './admin-management.component';
import {ReactiveFormsModule} from "@angular/forms";
import {AdminManagementRoutingModule} from "./admin-management-routing.module";
import { OAuthLoginComponent } from './components/oauth-login/o-auth-login.component';
import {AuthenticationApi} from "../../shared/authentication.api";
import {OAuthAuthenticationService} from "./shared/o-auth-authentication.service";
import {CaasAdminModule} from "../../caas-admin.module";
import {CardModule, FormModule} from "@coreui/angular";
import {NavProviderApi} from "../../shared/nav-provider.api";
import {ManagementNavProviderService} from "./shared/management-nav-provider.service";

@NgModule({
  declarations: [
    AdminManagementComponent,
    OAuthLoginComponent
  ],
  imports: [
    CaasAdminModule,
    AdminManagementRoutingModule,
    ReactiveFormsModule,
    FormModule,
    CardModule
  ],
  providers: [
    { provide: AuthenticationApi, useClass: OAuthAuthenticationService },
    { provide: NavProviderApi, useClass: ManagementNavProviderService }
  ]
})
export class AdminManagementModule { }
