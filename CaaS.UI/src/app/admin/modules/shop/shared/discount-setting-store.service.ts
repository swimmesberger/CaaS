import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {TenantIdService} from "../../../../shared/tenant-id.service";
import {Observable, throwError} from "rxjs";
import {environment} from "../../../../../environments/environment";
import {DiscountSettingDto} from "./models/discountSettingDto";
import {AppKeyAuthenticationService} from "./app-key-authentication.service";
import {ShopDto} from "../../../shared/models/shopDto";
import {v4 as uuidv4} from "uuid";

@Injectable({
  providedIn: 'root'
})
export class DiscountSettingStoreService {
  constructor(private httpClient: HttpClient,
              private tenantIdService: TenantIdService,
              private authenticationService: AppKeyAuthenticationService) {  }

  public getDiscountSettings(): Observable<Array<DiscountSettingDto>> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getDiscountSettings.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling getDiscountSettings.'));
    }
    return this.httpClient.get<Array<DiscountSettingDto>>(`${environment.url}/discount`, {
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

  public getDiscountSettingById(settingId: string): Observable<DiscountSettingDto> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling getDiscountSettingById.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling getDiscountSettingById.'));
    }
    return this.httpClient.get<DiscountSettingDto>(`${environment.url}/discount/${settingId}`, {
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

  public createDiscountSetting(setting: DiscountSettingDto): Observable<void> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling createDiscountSetting.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling createDiscountSetting.'));
    }
    if (!setting.rule) {
      throw new Error('Required parameter rule was null or undefined when calling createDiscountSetting.');
    }
    if (!setting.action) {
      throw new Error('Required parameter action was null or undefined when calling createDiscountSetting.');
    }
    if (!setting.id) {
      setting.id = uuidv4();
    }
    return this.httpClient.post<void>(`${environment.url}/discount`, setting,{
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

  public updateDiscountSetting(setting: DiscountSettingDto): Observable<DiscountSettingDto> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling updateDiscountSetting.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling updateDiscountSetting.'));
    }
    return this.httpClient.put<DiscountSettingDto>(`${environment.url}/discount/${setting.id}`, setting,{
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

  public deleteDiscountSettings(settingId: string): Observable<void> {
    const xTenantId = this.tenantIdService.tenantId;
    if (xTenantId === null || xTenantId === undefined) {
      return throwError(() => new Error('Required parameter xTenantId was null or undefined when calling deleteDiscountSettings.'));
    }
    const appKey = this.authenticationService.appKey;
    if (appKey === null || appKey === undefined) {
      return throwError(() => new Error('Required parameter appKey was null or undefined when calling deleteDiscountSettings.'));
    }
    return this.httpClient.delete<void>(`${environment.url}/discount/${settingId}`, {
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
