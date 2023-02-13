import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {ShopDto} from "../../../shared/models/shopDto";
import {environment} from "../../../../../environments/environment";
import { v4 as uuidv4 } from 'uuid';

@Injectable({
  providedIn: 'root'
})
export class ShopStoreService {
  constructor(private httpClient: HttpClient) {  }

  public getShops(): Observable<Array<ShopDto>> {
    return this.httpClient.get<Array<ShopDto>>(`${environment.url}/shopadministration`, {
      headers: {
        'Accept': [
          'application/json',
          'text/json',
          'application/problem+json'
        ]
      }
    });
  }

  public getShopById(shopId: string): Observable<ShopDto> {
    return this.httpClient.get<ShopDto>(`${environment.url}/shopadministration/${shopId}`, {
      headers: {
        'Accept': [
          'application/json',
          'text/json',
          'application/problem+json'
        ]
      }
    });
  }

  public createShop(shop: ShopDto): Observable<void> {
    if (!shop.shopAdmin) {
      throw new Error('Required parameter shopAdmin was null or undefined when calling createShop.');
    }
    if (!shop.id) {
      shop.id = uuidv4();
    }
    if (!shop.shopAdmin?.id) {
      shop.shopAdmin.id = uuidv4();
    }
    return this.httpClient.post<void>(`${environment.url}/shopadministration`, shop,{
      headers: {
        'Accept': [
          'application/json',
          'text/json',
          'application/problem+json'
        ]
      }
    });
  }

  public updateShop(shop: ShopDto): Observable<ShopDto> {
    return this.httpClient.put<ShopDto>(`${environment.url}/shopadministration/${shop.id}`, shop,{
      headers: {
        'Accept': [
          'application/json',
          'text/json',
          'application/problem+json'
        ]
      }
    });
  }

  public deleteShop(shopId: string): Observable<void> {
    return this.httpClient.delete<void>(`${environment.url}/shopadministration/${shopId}`, {
      headers: {
        'Accept': [
          'application/json',
          'text/json',
          'application/problem+json'
        ]
      }
    });
  }
}
