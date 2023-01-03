import { Component } from '@angular/core';
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";

@Component({
  selector: 'app-cart-widget',
  templateUrl: './cart-widget.component.html',
  styleUrls: ['./cart-widget.component.scss'],
  host: {
    class: 'navbar-tool ms-3'
  }
})
export class CartWidgetComponent {
  protected $cart: Observable<CartDto>;

  constructor(private cartService: CartService) {
    this.$cart = cartService.$cart;
  }

  getProductCount(cart: CartDto): number {
    let sum = 0;
    for (let item of cart.items!) {
      sum += item?.amount ?? 0;
    }
    return sum;
  }
}
