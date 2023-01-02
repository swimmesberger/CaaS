import { Component, OnInit } from '@angular/core';
import {CartService} from "../../shared/cart/cart.service";
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
  host: {
    class: 'container pb-5 mb-2 mb-md-4'
  }
})
export class CheckoutComponent implements OnInit {
  protected $cart: Observable<CartDto>;

  constructor(private cartService: CartService) {
    this.$cart = cartService.$cart;
  }

  ngOnInit(): void {
  }

}
