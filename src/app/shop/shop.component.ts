import {Component, OnDestroy} from '@angular/core';
import {ProductSearchService} from "./shared/product/product-search.service";

@Component({
  selector: 'shop-root',
  template: `
    <main class="page-wrapper">
      <router-outlet></router-outlet>
    </main>
  `
})
export class ShopComponent implements OnDestroy {
  constructor(private productSearchService: ProductSearchService) { }

  ngOnDestroy(): void {
    this.productSearchService.setSearchText(null)
  }
}
