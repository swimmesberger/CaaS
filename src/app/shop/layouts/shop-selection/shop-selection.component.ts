import {Component, ViewEncapsulation} from '@angular/core';
import {ShopService} from "../../shared/shop/shop.service";
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
  constructor(private shopService: ShopService) {
    this.$shops = shopService.getShops();
  }
}
