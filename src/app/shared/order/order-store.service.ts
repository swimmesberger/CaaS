import { Injectable } from '@angular/core';
import {Observable, of} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class OrderStoreService {
  constructor() { }

  public getCountries(): Observable<Array<string>> {
    return of(['Australia','Canada','France','Germany','Switzerland','USA']);
  }
}
