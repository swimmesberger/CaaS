import { Component } from '@angular/core';
import {StepInfoDto} from "../checkout-steps/step-info-dto";
import {CheckoutComponent} from "../checkout/checkout.component";
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";
import {OrderService} from "../../shared/order/order.service";
import {CustomerWithAddressDto} from "../../shared/order/models/customerWithAddressDto";

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss'],
  host: {
    class: "container pb-5 mb-2 mb-md-4"
  }
})
export class CheckoutReviewComponent {
  protected steps: Array<StepInfoDto> = CheckoutComponent.Steps;
  protected $cart: Observable<CartDto>;
  protected customerData: CustomerWithAddressDto;

  constructor(private cartService: CartService,
              private orderService: OrderService) {
    this.$cart = cartService.$cart;
    this.customerData = orderService.customerData ?? <CustomerWithAddressDto>{
      customer: {},
      address: {}
    };
  }

  get creditCardNumber(): string {
    return `**** **** **** ${this.customerData.customer?.creditCardNumber?.slice(-4) ?? '0000'}`;
  }

  get orderNumber(): string {
    return '1234';
  }
}
