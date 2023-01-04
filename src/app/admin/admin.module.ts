import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminComponent } from './admin.component';
import {ReactiveFormsModule} from "@angular/forms";
import {AdminRoutingModule} from "./admin-routing.module";
import {PageNotFoundComponent} from "../components/page-not-found/page-not-found.component";
import { LoginComponent } from './components/login/login.component';

@NgModule({
  declarations: [
    AdminComponent,
    PageNotFoundComponent,
    LoginComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    ReactiveFormsModule
  ]
})
export class AdminModule { }
