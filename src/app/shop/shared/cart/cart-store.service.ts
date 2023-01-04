import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {first, Observable, switchMap, throwError} from "rxjs";
import {environment} from "../../../../environments/environment";
import {CartDto} from "./models/cartDto";
import {TenantIdService} from "../shop/tenant-id.service";

@Injectable({
  providedIn: 'root'
})
export class CartStoreService {
  constructor(private httpClient: HttpClient,
              private tenantIdService: TenantIdService) {  }

  public getCartById(cartId: string): Observable<CartDto> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getCartById.'));
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

    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getCartById.'));
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
    })
  }
}
