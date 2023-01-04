import { Injectable } from '@angular/core';
import {BehaviorSubject, distinctUntilChanged, filter, Observable} from "rxjs";
import {ActivatedRoute, NavigationEnd, Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class TenantIdService {
  private readonly _$tenantId: BehaviorSubject<string | null>;
  private readonly _$tenantIdObs: Observable<string | null>;

  constructor(private router: Router) {
    this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe(event =>    {
      let currentRoute: ActivatedRoute | null = this.router.routerState.root;
      do {
        const routeSnapshot = currentRoute.snapshot;
        if(routeSnapshot.data['tenantUser']) {
          const shopId = currentRoute.snapshot.params['shopId'];
          if (shopId) {
            this.setTenantId(shopId);
            break;
          }
        }
        currentRoute = currentRoute.firstChild;
      } while (currentRoute);
    });
    this._$tenantId = new BehaviorSubject<string | null>(null);
    this._$tenantIdObs = this._$tenantId.asObservable().pipe(
      distinctUntilChanged()
    );
  }

  get tenantUrl() {
    return `/shop/${this.tenantId}`;
  }

  get baseUrl() {
    return '';
  }

  get $tenantId(): Observable<string | null> {
    return this._$tenantIdObs;
  }

  get tenantId(): string | null {
    return this._$tenantId.value;
  }

  setTenantId(tenantId: string | null): void {
    this._$tenantId.next(tenantId);
  }
}
