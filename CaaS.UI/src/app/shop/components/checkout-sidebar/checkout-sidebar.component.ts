import {Component, Input} from '@angular/core';
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";
import {ProductService} from "../../shared/product/product.service";
import {CartItemDto} from "../../shared/cart/models/cartItemDto";

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

  constructor(private cartService: CartService,
              private productService: ProductService) {
    this.$cart = cartService.$cart;
  }

  productImage(cartItem: CartItemDto, width: number, height: number): string {
    return this.productService.productImage(cartItem.product, width, height);
  }
}
