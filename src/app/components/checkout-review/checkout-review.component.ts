import { Component } from '@angular/core';
import {StepInfoDto} from "../checkout-steps/step-info-dto";
import {CheckoutComponent} from "../checkout/checkout.component";
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";
import {OrderService} from "../../shared/order/order.service";
import {CustomerWithAddressDto} from "../../shared/order/models/customerWithAddressDto";
import {OrderDto} from "../../shared/order/models/orderDto";
import {Router} from "@angular/router";

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
  protected createdOrder?: OrderDto;
  protected isLoading: boolean = false;
  protected error: Set<string> = new Set<string>();

  constructor(private router: Router,
              private cartService: CartService,
              private orderService: OrderService) {
    this.$cart = cartService.$cart;
    this.customerData = orderService.customerData ?? <CustomerWithAddressDto>{
      customer: {},
      address: {}
    };
  }

  async completeOrder(e: Event): Promise<void> {
    this.isLoading = true;
    this.error.clear();
    try {
      e.preventDefault();
      e.stopPropagation();
      this.createdOrder = await this.orderService.createOrder(this.customerData);
      // noinspection ES6MissingAwait
      this.router.navigate(['/checkout/complete'], { queryParams: { orderNumber: this.orderNumber }});
    } catch (ex) {
      this.error.add('serverError');
    } finally {
      this.isLoading = false;
    }
  }

  get creditCardNumber(): string {
    return `**** **** **** ${this.customerData.customer?.creditCardNumber?.slice(-4) ?? '0000'}`;
  }

  get orderNumber(): string {
    return this.createdOrder?.orderNumber ?? '';
  }

  hasError(key: string | undefined = undefined): boolean {
    if (key) {
      return this.error.has(key);
    }
    return this.error.size > 0;
  }
}
