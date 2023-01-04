import { Injectable } from '@angular/core';
import {Observable, of} from "rxjs";
import {environment} from "../../../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {AddressDto} from "./models/addressDto";
import {OrderDto} from "./models/orderDto";

@Injectable({
  providedIn: 'root'
})
export class OrderStoreService {
  constructor(private httpClient: HttpClient) {  }

  public getCountries(): Observable<Array<string>> {
    return of(['Australia','Canada','France','Germany','Switzerland','USA']);
  }

  public createOrder(cartId: string, billingAddress: AddressDto): Observable<OrderDto> {
    if (cartId === null || cartId === undefined) {
      throw new Error('Required parameter cartId was null or undefined when calling updateCart.');
    }
    let xTenantId = environment.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      throw new Error('Required parameter xTenantId was null or undefined when calling productGet.');
    }

    return this.httpClient.post<OrderDto>(`${environment.url}/order`, {
      cartId: cartId,
      BillingAddress: billingAddress
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
