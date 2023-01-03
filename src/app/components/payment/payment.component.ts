import {Component, ViewChild} from '@angular/core';
import {OrderService} from "../../shared/order/order.service";
import {StepInfoDto} from "../checkout-steps/step-info-dto";
import {CheckoutComponent} from "../checkout/checkout.component";
import {CreditcardComponent} from "../creditcard/creditcard.component";
import {CreditCardDataDto} from "../../shared/order/models/customerWithAddressDto";

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss'],
  host: {
    class: 'container pb-5 mb-2 mb-md-4'
  }
})
export class PaymentComponent {
  @ViewChild("creditcardComponent") creditcardComponent!: CreditcardComponent;
  protected steps: Array<StepInfoDto> = CheckoutComponent.Steps;

  constructor(private orderService: OrderService) {  }

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
