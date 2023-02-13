import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {TenantIdService} from "./shared/tenant-id.service";
@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ],
  exports: [],
  providers: [TenantIdService]
})
export class CaasCommonModule { }
