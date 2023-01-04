import { NgModule } from '@angular/core';
import { AdminComponent } from './admin.component';
import {ReactiveFormsModule} from "@angular/forms";
import {AdminRoutingModule} from "./admin-routing.module";
import { LoginComponent } from './components/login/login.component';
import {CaasCommonModule} from "../caas-common.module";
import {DefaultFooterComponent, DefaultHeaderComponent, DefaultLayoutComponent} from "./layouts/default-layout";
import {PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface, PerfectScrollbarModule} from "ngx-perfect-scrollbar";
import {
  AvatarModule,
  BadgeModule,
  BreadcrumbModule, ButtonModule,
  DropdownModule,
  GridModule,
  HeaderModule,
  NavModule,
  SidebarModule, UtilitiesModule
} from "@coreui/angular";
import {IconModule} from "@coreui/icons-angular";

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true,
};

@NgModule({
  declarations: [
    AdminComponent,
    LoginComponent,
    DefaultLayoutComponent,
    DefaultHeaderComponent,
    DefaultFooterComponent
  ],
  imports: [
    CaasCommonModule,
    AdminRoutingModule,
    ReactiveFormsModule,
    SidebarModule,
    PerfectScrollbarModule,
    GridModule,
    BadgeModule,
    NavModule,
    HeaderModule,
    BreadcrumbModule,
    DropdownModule,
    AvatarModule,
    IconModule,
    ButtonModule,
    UtilitiesModule
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG,
    },
  ]
})
export class AdminModule { }
