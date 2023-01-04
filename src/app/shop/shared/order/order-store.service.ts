import { Injectable } from '@angular/core';
import {first, Observable, of, switchMap, throwError} from "rxjs";
import {environment} from "../../../../environments/environment"
import {HttpClient} from "@angular/common/http";
import {AddressDto} from "./models/addressDto";
import {OrderDto} from "./models/orderDto";
import {CustomerDto} from "../cart/models/customerDto";
import {TenantIdService} from "../shop/tenant-id.service";

@Injectable({
  providedIn: 'root'
})
export class OrderStoreService {
  constructor(private httpClient: HttpClient,
              private tenantIdService: TenantIdService) {  }

  public getCountries(): Observable<Array<string>> {
    return of(['Australia','Canada','France','Germany','Switzerland','USA']);
  }

  public createOrder(cartId: string, billingAddress: AddressDto, customer: CustomerDto): Observable<OrderDto> {
    if (cartId === null || cartId === undefined) {
      throw new Error('Required parameter cartId was null or undefined when calling updateCart.');
    }
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getCartById.'));
    }
    return this.httpClient.post<OrderDto>(`${environment.url}/order`, {
      cartId: cartId,
      billingAddress: billingAddress,
      customer: customer
    }, {
      headers: {
        'X-tenant-id': xTenantId,
        'Accept': [
          'application/json',
          'text/json',
          'application/problem+json'
        ]
      }
    });
  }
}
