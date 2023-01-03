import { Injectable } from '@angular/core';
import {Observable, of} from "rxjs";
import {CartDto} from "../cart/models/cartDto";
import {environment} from "../../../environments/environment";
import {HttpParams} from "@angular/common/http";
import {AddressDto} from "./models/addressDto";

@Injectable({
  providedIn: 'root'
})
export class OrderStoreService {
  constructor() { }

  public getCountries(): Observable<Array<string>> {
    return of(['Australia','Canada','France','Germany','Switzerland','USA']);
  }

  public createOrder(cart: CartDto, billing: AddressDto): Observable<void> {
    if (cart === null || cart === undefined) {
      throw new Error('Required parameter cart was null or undefined when calling updateCart.');
    }
    if (cart.id === null || cart.id === undefined) {
      throw new Error('Required parameter cartId was null or undefined when calling updateCart.');
    }
    let xTenantId = environment.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      throw new Error('Required parameter xTenantId was null or undefined when calling productGet.');
    }

    let queryParameters = new HttpParams();
    return this.httpClient.post<void>(`${environment.url}/cart/${encodeURIComponent(cart.id!)}`, cart, {
      params: queryParameters,
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
