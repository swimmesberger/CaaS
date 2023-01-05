import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../../../environments/environment";
import {ShopDto} from "../../../shared/models/shopDto";

@Injectable({
  providedIn: 'root'
})
export class ShopStoreService {
  constructor(private httpClient: HttpClient) {  }

  public getShopByAdmin(shopAdminEmail: string, appKey: string): Observable<ShopDto> {
    return this.httpClient.get<ShopDto>(`${environment.url}/shopadministration/adminmail/${shopAdminEmail}`, {
      headers: {
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
