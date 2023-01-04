import { Injectable } from '@angular/core';
import {BehaviorSubject, debounceTime, distinctUntilChanged, Observable, ReplaySubject, Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ProductSearchService {
  private readonly _$searchText: Subject<string | null>;
  private readonly _$searchTextObs: Observable<string | null>;

  constructor() {
    this._$searchText = new ReplaySubject<string | null>(1);
    this._$searchTextObs = this._$searchText.asObservable().pipe(
      debounceTime(300),
      distinctUntilChanged()
    );
  }

  get searchText(): Observable<string | null> {
    return this._$searchTextObs;
  }

  setSearchText(text: string | null): void {
    this._$searchText.next(text);
  }
}
