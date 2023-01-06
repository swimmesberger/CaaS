import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {lastValueFrom, Observable, of, Subscription, switchMap} from "rxjs";
import {ShopDto} from "../../../../shared/models/shopDto";
import {ShopStoreService} from "../../shared/shop-store.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-shop-edit',
  templateUrl: './shop-edit.component.html',
  styleUrls: ['./shop-edit.component.scss']
})
export class ShopEditComponent implements OnInit, OnDestroy {
  @Input() shopId: string | null;
  shopForm: FormGroup<ShopForm|undefined>;

  private _shopSub: Subscription | undefined;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private shopService: ShopStoreService) {
    this.shopId = null;
    this.shopForm = new FormGroup<ShopForm|undefined>(<ShopForm>{
      id: new FormControl<string | undefined>(undefined),
      name: new FormControl<string | undefined>(undefined, Validators.required),
      appKey: new FormControl<string| undefined>(undefined, Validators.required),
      shopAdmin: new FormGroup<ShopAdminForm>(<ShopAdminForm>{
        id: new FormControl<string | undefined>(undefined),
        name: new FormControl<string | undefined>(undefined, Validators.required),
        eMail: new FormControl<string | undefined>(undefined,
          Validators.compose([Validators.required, Validators.email])),
        shopId: new FormControl<string | undefined>(undefined),
        concurrencyToken: new FormControl<string | undefined>(undefined),
      }),
      cartLifetimeMinutes: new FormControl<number | undefined>(120,
        Validators.compose([Validators.required, Validators.min(0)])),
      concurrencyToken: new FormControl<string | undefined>(undefined),
    });
  }

  ngOnInit(): void {
    let $shop: Observable<ShopDto | null>;
    if (this.shopId == null) {
      $shop = this.route.params.pipe(switchMap(params => {
        const shopId = params['shopId'];
        if (!shopId) return of(null);
        return this.shopService.getShopById(shopId);
      }));
    } else {
      $shop = this.shopService.getShopById(this.shopId);
    }
    this._shopSub = $shop.subscribe(shop => {
      if (shop === null) {
        this.shopId = null;
        this.shopForm.reset();
        return;
      }
      this.shopId = shop.id ?? null;
      // @ts-ignore
      this.shopForm.setValue(shop);
    });
  }

  ngOnDestroy() {
    this._shopSub?.unsubscribe();
  }

  async save(e: Event): Promise<void> {
    e.preventDefault();
    e.stopPropagation();

    // @ts-ignore
    const shop: ShopDto = {
      ...this.shopForm.value
    }
    if(this.shopId == null) {
      await lastValueFrom(this.shopService.createShop(shop));
    } else {
      await lastValueFrom(this.shopService.updateShop(shop));
    }
    // noinspection ES6MissingAwait
    this.router.navigate(['/admin/shops']);
  }
}

interface ShopAdminForm {
  id: FormControl<string|undefined>,
  name: FormControl<string|undefined>,
  eMail: FormControl<string|undefined>,
  shopId: FormControl<string|undefined>,
  concurrencyToken: FormControl<string|undefined>
}

interface ShopForm {
  id: FormControl<string|undefined>;
  name: FormControl<string|undefined>;
  appKey: FormControl<string|undefined>;
  shopAdmin: FormGroup<ShopAdminForm|undefined>;
  cartLifetimeMinutes: FormControl<number|undefined>,
  concurrencyToken: FormControl<string|undefined>
}
