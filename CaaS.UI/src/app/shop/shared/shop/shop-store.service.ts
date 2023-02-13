import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../../environments/environment";
import {ShopMinimalDto} from "./shopMinimalDto";

@Injectable({
  providedIn: 'root'
})
export class ShopStoreService {
  constructor(private httpClient: HttpClient) {  }

  public getShops(): Observable<Array<ShopMinimalDto>> {
    return this.httpClient.get<Array<ShopMinimalDto>>(`${environment.url}/shop`, {
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
