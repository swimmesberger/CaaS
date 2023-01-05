import {Component, ViewEncapsulation} from '@angular/core';
import {ShopStoreService} from "../../shared/shop/shop-store.service";
import {Observable} from "rxjs";
import {ShopMinimalDto} from "../../shared/shop/shopMinimalDto";
import {RouterLinkWithHref} from "@angular/router";
import {AsyncPipe, NgForOf, NgIf} from "@angular/common";

@Component({
  standalone: true,
  selector: 'app-shop-selection',
  templateUrl: './shop-selection.component.html',
  imports: [
    RouterLinkWithHref,
    AsyncPipe,
    NgForOf,
    NgIf
  ],
  styleUrls: ['./shop-selection.component.scss']
})
export class ShopSelectionComponent {
  $shops: Observable<Array<ShopMinimalDto>>;
  constructor(private shopService: ShopStoreService) {
    this.$shops = shopService.getShops();
  }
}
