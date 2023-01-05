import { NgModule } from '@angular/core';
import {DefaultFooterComponent, DefaultHeaderComponent, DefaultLayoutComponent} from "./layouts/default-layout";
import {CaasCommonModule} from "../caas-common.module";
import {
  AvatarModule,
  BadgeModule,
  BreadcrumbModule, ButtonModule,
  DropdownModule,
  GridModule,
  HeaderModule,
  SidebarModule, UtilitiesModule
} from "@coreui/angular";
import {PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface, PerfectScrollbarModule} from "ngx-perfect-scrollbar";
import {IconModule} from "@coreui/icons-angular";
import {RouterOutlet} from "@angular/router";

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true,
};

@NgModule({
  declarations: [
    DefaultLayoutComponent,
    DefaultHeaderComponent,
    DefaultFooterComponent,
  ],
  imports: [
    CaasCommonModule,
    SidebarModule,
    PerfectScrollbarModule,
    GridModule,
    BadgeModule,
    HeaderModule,
    BreadcrumbModule,
    DropdownModule,
    AvatarModule,
    IconModule,
    ButtonModule,
    UtilitiesModule,
    RouterOutlet,
  ],
  exports: [
    DefaultLayoutComponent,
    DefaultHeaderComponent,
    DefaultFooterComponent
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG,
    }
  ]
})
export class CaasAdminModule { }
