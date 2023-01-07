import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable, throwError} from "rxjs";
import {environment} from "../../../../../environments/environment";
import {ShopDto} from "../../../shared/models/shopDto";
import {TenantIdService} from "../../../../shared/tenant-id.service";

@Injectable({
  providedIn: 'root'
})
export class ShopStoreService {
  constructor(private httpClient: HttpClient,
              private tenantIdService: TenantIdService) {  }

  public getShopByAdmin(shopAdminEmail: string, appKey: string): Observable<ShopDto> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getShopByAdmin.'));
    }
    return this.httpClient.get<ShopDto>(`${environment.url}/shopadministration/adminmail/${shopAdminEmail}`, {
      headers: {
        'X-tenant-id': xTenantId,
        'X-app-key': appKey,
        'Accept': [
          'application/json',
          'text/json',
          'application/problem+json'
        ]
      }
    });
  }
}
