import {Component, ViewEncapsulation} from '@angular/core';
import {ShopStoreService} from "../../shop/shared/shop/shop-store.service";
import {Observable} from "rxjs";
import {ShopMinimalDto} from "../../shop/shared/shop/shopMinimalDto";
import {RouterLinkWithHref} from "@angular/router";
import {AsyncPipe, NgForOf, NgIf} from "@angular/common";

@Component({
  standalone: true,
  selector: 'app-shop-selection',
  templateUrl: './home.component.html',
  imports: [
    RouterLinkWithHref,
    AsyncPipe,
    NgForOf,
    NgIf
  ],
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  $shops: Observable<Array<ShopMinimalDto>>;
  constructor(private shopService: ShopStoreService) {
    this.$shops = shopService.getShops();
  }
}
