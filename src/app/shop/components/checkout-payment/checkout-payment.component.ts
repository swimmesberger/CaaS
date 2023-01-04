import {Component, ViewChild} from '@angular/core';
import {OrderService} from "../../shared/order/order.service";
import {StepInfoDto} from "../checkout-steps/step-info-dto";
import {CheckoutComponent} from "../checkout/checkout.component";
import {CreditcardComponent} from "../creditcard/creditcard.component";
import {CreditCardDataDto} from "../../shared/order/models/customerWithAddressDto";
import {TenantIdService} from "../../shared/shop/tenant-id.service";

@Component({
  selector: 'app-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss'],
  host: {
    class: 'container pb-5 mb-2 mb-md-4'
  }
})
export class CheckoutPaymentComponent {
  @ViewChild("creditcardComponent") creditcardComponent!: CreditcardComponent;
  protected steps: Array<StepInfoDto> = CheckoutComponent.Steps;

  constructor(private orderService: OrderService,
              protected tenantService: TenantIdService) {  }

  get creditCardData(): CreditCardDataDto | null {
    return this.orderService.customerData?.creditCard ?? null
  }

  saveCustomerData(): void {
    const creditCardData = this.creditcardComponent.creditCardData;
    if(!creditCardData) return;
    this.orderService.customerData = {
      ...this.orderService.customerData,
      creditCard: creditCardData,
    };
  }
}
