import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {OrderStoreService} from "./order-store.service";
import {CustomerWithAddressDto} from "./models/customerWithAddressDto";

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private _customerData?: CustomerWithAddressDto;

  constructor(private orderStoreService: OrderStoreService) { }

  public getCountries(): Observable<Array<string>> {
    return this.orderStoreService.getCountries();
  }

  set customerData(data: CustomerWithAddressDto | undefined) {
    this._customerData = data;
  }

  get customerData(): CustomerWithAddressDto | undefined {
     return this._customerData;
  }
}
