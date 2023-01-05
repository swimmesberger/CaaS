import { Component } from '@angular/core';
import {StepInfoDto} from "../checkout-steps/step-info-dto";
import {CheckoutComponent} from "../checkout/checkout.component";
import {map, Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";
import {OrderService} from "../../shared/order/order.service";
import {CustomerWithAddressDto} from "../../shared/order/models/customerWithAddressDto";
import {OrderDto} from "../../shared/order/models/orderDto";
import {ActivatedRoute, Router} from "@angular/router";
import {ProductService} from "../../shared/product/product.service";
import {CartItemDto} from "../../shared/cart/models/cartItemDto";
import {CaasDuplicateCustomerEmailError} from "../../shared/errors/caasDuplicateCustomerEmailError";
import {CaasPaymentError} from "../../shared/errors/caasPaymentError";
import {TenantIdService} from "../../../shared/tenant-id.service";

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss'],
  host: {
    class: "container pb-5 mb-2 mb-md-4"
  }
})
export class CheckoutReviewComponent {
  protected steps: Array<StepInfoDto>;
  protected $cart: Observable<CartDto>;
  protected $customerData: Observable<CustomerWithAddressDto>;
  protected createdOrder?: OrderDto;
  protected isLoading: boolean;
  protected error: Set<string>;

  constructor(private router: Router,
              private activatedRoute: ActivatedRoute,
              private cartService: CartService,
              private orderService: OrderService,
              private productService: ProductService,
              protected tenantService: TenantIdService) {
    this.steps = CheckoutComponent.Steps;
    this.$cart = cartService.$cart;
    this.$customerData = orderService.$customerData.pipe(map(v => v ?? <CustomerWithAddressDto>{
      customer: {},
      address: {}
    }));
    this.isLoading = false;
    this.error = new Set<string>();
  }

  async completeOrder(e: Event, customerData: CustomerWithAddressDto): Promise<void> {
    this.isLoading = true;
    this.error.clear();
    try {
      e.preventDefault();
      e.stopPropagation();
      await this.completeOrderImpl(customerData);
    } catch (ex) {
      if (ex instanceof CaasDuplicateCustomerEmailError) {
        this.error.add('serverCustomerEmailError');
      } if (ex instanceof CaasPaymentError) {
        this.error.add('serverPaymentError');
      } else {
        this.error.add('serverGenericError');
      }
    } finally {
      this.isLoading = false;
    }
  }

  private async completeOrderImpl(customerData: CustomerWithAddressDto): Promise<void> {
    this.createdOrder = await this.orderService.createOrder(customerData);
    // noinspection ES6MissingAwait
    this.router.navigate([this.tenantService.tenantUrl + '/checkout/complete'], {
      queryParams: { orderNumber: this.orderNumber },
      relativeTo: this.activatedRoute
    });
  }

  getCreditCardNumber(customerData: CustomerWithAddressDto): string {
    return `**** **** **** ${customerData.customer?.creditCardNumber?.slice(-4) ?? '0000'}`;
  }

  get orderNumber(): string {
    return this.createdOrder?.orderNumber ?? '';
  }

  hasError(): boolean {
    return this.error.size > 0;
  }

  getErrorMessage() : string {
    if (this.error.has('serverCustomerEmailError')) {
      return $localize `:@@checkoutCompleteServerCustomerEmailError:The provided E-Mail address is already taken. Please choose a different E-Mail.`;
    }
    if (this.error.has('serverPaymentError')) {
      return $localize `:@@checkoutCompleteServerPaymentError:The provided credit card can't be charged. Please choose a different card.`;
    }
    return $localize `:@@checkoutCompleteErrorServerError:Failed to process order. Try it later again.`;
  }

  productImage(cartItem: CartItemDto, width: number, height: number): string {
    return this.productService.productImage(cartItem.product, width, height);
  }
}
