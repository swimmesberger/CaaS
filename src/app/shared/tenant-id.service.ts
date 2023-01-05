import { Injectable } from '@angular/core';
import {BehaviorSubject, distinctUntilChanged, filter, Observable} from "rxjs";
import {ActivatedRouteSnapshot, NavigationEnd, Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class TenantIdService {
  private readonly _$tenantId: BehaviorSubject<string | null>;
  private readonly _$tenantIdObs: Observable<string | null>;

  constructor(private router: Router) {
    this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe(event => {
      const activatedRoute = this.router.routerState.root;
      this.setTenantId(this.getTenantId(activatedRoute.snapshot));
    });
    this._$tenantId = new BehaviorSubject<string | null>(this.tenantId);
    this._$tenantIdObs = this._$tenantId.asObservable().pipe(
      distinctUntilChanged()
    );
  }

  get tenantUrl() {
    return this.getTenantUrl(this.router.routerState.root.snapshot)
  }

  get baseUrl() {
    return '';
  }

  get $tenantId(): Observable<string | null> {
    return this._$tenantIdObs;
  }

  get tenantId(): string | null {
    return this.getTenantId(this.router.routerState.root.snapshot);
  }

  setTenantId(tenantId: string | null): void {
    this._$tenantId.next(tenantId);
  }

  getTenantUrl(activatedRoute: ActivatedRouteSnapshot) {
    return `/shop/${this.getTenantId(activatedRoute)}`;
  }

  getTenantId(activatedRoute: ActivatedRouteSnapshot) : string | null {
    if (this._$tenantId && this._$tenantId.value !== null) {
      return this._$tenantId.value;
    }
    let currentRoute: ActivatedRouteSnapshot | null = activatedRoute;
    do {
      if(currentRoute.data['tenantUser']) {
        const shopId = currentRoute.params['shopId'];
        if (shopId) {
          return shopId;
        }
      }
      currentRoute = currentRoute.firstChild;
    } while (currentRoute);
    return null;
  }
}
