import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import {CartDto} from "./models/cartDto";

@Injectable({
  providedIn: 'root'
})
export class CartStoreService {
  constructor(private httpClient: HttpClient) {  }

  public getCartById(cartId: string): Observable<CartDto> {
    let xTenantId = environment.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      throw new Error('Required parameter xTenantId was null or undefined when calling getCartById.');
    }
    return this.httpClient.get<CartDto>(`${environment.url}/cart/${encodeURIComponent(cartId)}`, {
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

  public updateCart(cart: CartDto): Observable<void> {
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

    return this.httpClient.post<void>(`${environment.url}/cart/${encodeURIComponent(cart.id!)}`, cart, {
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
