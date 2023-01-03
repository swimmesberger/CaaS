import {Component, Input, OnInit} from '@angular/core';
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";

@Component({
  selector: 'app-checkout-sidebar',
  templateUrl: './checkout-sidebar.component.html',
  styleUrls: ['./checkout-sidebar.component.scss'],
  host: {
    class: "bg-white rounded-3 shadow-lg p-4 ms-lg-auto"
  }
})
export class CheckoutSidebarComponent {
  @Input() showProducts: boolean = true;
  protected $cart: Observable<CartDto>;

  constructor(private cartService: CartService) {
    this.$cart = cartService.$cart;
  }
}
