import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import { environment } from 'src/environments/environment';
import {first, Observable, switchMap, throwError} from "rxjs";
import {ProductMinimalDtoPagedResult} from "./models/productMinimalDtoPagedResult";
import {HttpUtil} from "../http-util";
import {ParsedPaginationToken} from "./models/parsedPaginationToken";
import {ProductDetailDto} from "./models/productDetailDto";
import {TenantIdService} from "../shop/tenant-id.service";

@Injectable({
  providedIn: 'root'
})
export class ProductStoreService {
  constructor(private httpClient: HttpClient,
              private tenantIdService: TenantIdService) {  }

  /**
   * @param searchText
   * @param paginationToken
   * @param limit
   */
  public search(searchText?: string, paginationToken?: ParsedPaginationToken, limit?: number): Observable<ProductMinimalDtoPagedResult> {
    let queryParameters = new HttpParams();
    if (searchText !== undefined && searchText !== null) {
      queryParameters = HttpUtil.addToHttpParams(queryParameters, searchText, 'q');
    }
    if (paginationToken?.direction !== undefined && paginationToken?.direction !== null) {
      queryParameters = HttpUtil.addToHttpParams(queryParameters, paginationToken.direction, 'paginationDirection');
    }
    if (paginationToken?.reference !== undefined && paginationToken?.reference !== null) {
      queryParameters = HttpUtil.addToHttpParams(queryParameters, paginationToken.reference, '$skiptoken');
    }
    if (limit !== undefined && limit !== null) {
      queryParameters = HttpUtil.addToHttpParams(queryParameters, limit, 'limit');
    }
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getCartById.'));
    }
    return this.httpClient.get<ProductMinimalDtoPagedResult>(`${environment.url}/Product`, {
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

  /**
   * @param productId
   */
  public getProductById(productId: string): Observable<ProductDetailDto> {
    if (productId === null || productId === undefined) {
      throw new Error('Required parameter productId was null or undefined when calling productProductIdGet.');
    }
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getCartById.'));
    }
    return this.httpClient.get<ProductDetailDto>(`${environment.url}/product/${encodeURIComponent(productId)}`, {
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
