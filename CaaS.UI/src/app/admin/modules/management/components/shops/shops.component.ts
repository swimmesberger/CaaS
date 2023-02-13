import {Component, ElementRef, QueryList, ViewChildren} from '@angular/core';
import {ShopStoreService} from "../../shared/shop-store.service";
import {ShopDto} from "../../../../shared/models/shopDto";
import {lastValueFrom, Observable} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-shops',
  templateUrl: './shops.component.html',
  styleUrls: ['./shops.component.scss']
})
export class ShopsComponent {
  public $shops: Observable<Array<ShopDto>>;
  @ViewChildren("selectionCbElement") selectionCbElement!: QueryList<ElementRef>;
  isLoading: boolean = false;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private shopService: ShopStoreService) {
    this.$shops = shopService.getShops();
  }

  async deleteSelectedShops(e: Event): Promise<void> {
    e.preventDefault();
    e.stopPropagation();

    this.isLoading = true;
    try {
      for (let cbElement of this.selectionCbElement) {
        if (!(cbElement.nativeElement instanceof HTMLInputElement)) continue;
        const shopId = cbElement.nativeElement.getAttribute('data-shop-id');
        if (!shopId) continue;
        const isSelected: boolean = cbElement.nativeElement.checked;
        if (isSelected) {
          await lastValueFrom(this.shopService.deleteShop(shopId));
        }
      }
      await this.refreshComponent();
    } finally {
      this.isLoading = false;
    }
  }

  private async refreshComponent(): Promise<void> {
    const url = this.router.url;
    // refresh
    await this.router.navigateByUrl('/', { skipLocationChange: true })
    // noinspection ES6MissingAwait
    this.router.navigateByUrl(url);
  }
}
