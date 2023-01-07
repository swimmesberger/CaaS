import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {TenantIdService} from "../../../../shared/tenant-id.service";
import {Observable, throwError} from "rxjs";
import {environment} from "../../../../../environments/environment";
import {AppKeyAuthenticationService} from "./app-key-authentication.service";
import {MostSoldProductResultDto} from "./models/mostSoldProductResultDto";
import {OrderStatisticsResultDateAggregateDto} from "./models/orderStatisticsResultDateAggregateDto";
import {CartStatisticsResultDto} from "./models/cartStatisticsResultDto";
import {CouponStatisticsResultDto} from "./models/couponStatisticsResultDto";
import {CartStatisticsResultDateAggregateDto} from "./models/cartStatisticsResultDateAggregateDto";

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {
  constructor(private httpClient: HttpClient,
              private tenantIdService: TenantIdService,
              private authenticationService: AppKeyAuthenticationService) {  }

  getMostSoldProducts(from: Date, to: Date): Observable<Array<MostSoldProductResultDto>> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getMostSoldProducts.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling getMostSoldProducts.'));
    }
    return this.httpClient.get<Array<MostSoldProductResultDto>>(`${environment.url}/orderadministration/MostSoldProducts`, {
      params: new HttpParams({
        fromObject: {
          from: from.toISOString(),
          until: to.toISOString()
        }
      }),
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

  getOrderStatistics(from: Date, to: Date, aggregate: string): Observable<Array<OrderStatisticsResultDateAggregateDto>> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getMostSoldProducts.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling getMostSoldProducts.'));
    }
    return this.httpClient.get<Array<OrderStatisticsResultDateAggregateDto>>(`${environment.url}/orderadministration/orderstatisticsaggregatedbydate`, {
      params: new HttpParams({
        fromObject: {
          from: from.toISOString(),
          until: to.toISOString(),
          aggregate: aggregate
        }
      }),
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

  getCartStatistics(from: Date, to: Date | null, aggregate: string): Observable<Array<CartStatisticsResultDateAggregateDto>> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getMostSoldProducts.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling getMostSoldProducts.'));
    }
    let params: HttpParams = new HttpParams();
    params = params.set('from', from.toISOString());
    params = params.set('aggregate', aggregate);
    if (to !== null) {
      params = params.set('until', to.toISOString());
    }
    return this.httpClient.get<Array<CartStatisticsResultDateAggregateDto>>(`${environment.url}/cartadministration/cartstatisticsaggregatedbydate`, {
      params: params,
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

  getOverallCartStatistics(from: Date, to: Date | null = null): Observable<CartStatisticsResultDto> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getMostSoldProducts.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling getMostSoldProducts.'));
    }
    let params: HttpParams = new HttpParams();
    params = params.set('from', from.toISOString());
    if (to !== null) {
      params = params.set('until', to.toISOString());
    }
    return this.httpClient.get<CartStatisticsResultDto>(`${environment.url}/cartadministration/cartstatisticsoverall`, {
      params: params,
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

  getOverallCouponStatistics(from: Date, to: Date | null = null): Observable<CouponStatisticsResultDto> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getMostSoldProducts.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling getMostSoldProducts.'));
    }
    let params: HttpParams = new HttpParams();
    params = params.set('from', from.toISOString());
    if (to !== null) {
      params = params.set('until', to.toISOString());
    }
    return this.httpClient.get<CouponStatisticsResultDto>(`${environment.url}/couponadministration/couponstatisticsoverall`, {
      params: params,
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
