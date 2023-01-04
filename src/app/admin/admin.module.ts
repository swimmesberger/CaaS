import { NgModule } from '@angular/core';
import { AdminComponent } from './admin.component';
import {ReactiveFormsModule} from "@angular/forms";
import {AdminRoutingModule} from "./admin-routing.module";
import { LoginComponent } from './components/login/login.component';
import {CaasCommonModule} from "../caas-common.module";

@NgModule({
  declarations: [
    AdminComponent,
    LoginComponent
  ],
  imports: [
    CaasCommonModule,
    AdminRoutingModule,
    ReactiveFormsModule
  ]
})
export class AdminModule { }
