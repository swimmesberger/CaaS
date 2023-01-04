import { Component } from '@angular/core';
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";
import { StepInfoDto } from '../checkout-steps/step-info-dto';
import {OrderService} from "../../shared/order/order.service";
import {CustomerWithAddressDto} from "../../shared/order/models/customerWithAddressDto";

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
  host: {
    class: 'container pb-5 mb-2 mb-md-4'
  }
})
export class CheckoutComponent {
  public static readonly Steps: Array<StepInfoDto> = [
    {routeLink: "/shop/cart", routeTitle: $localize `:@@cartStepTitle:Cart`, routeIcon: 'cart'},
    {routeLink: "/shop/checkout", routeTitle: $localize `:@@checkoutStepTitle:Details`, routeIcon: 'person-circle'},
    {routeLink: "/shop/checkout/payment", routeTitle: $localize `:@@paymentStepTitle:Payment`, routeIcon: 'credit-card'},
    {routeLink: "/shop/checkout/review", routeTitle: $localize `:@@reviewStepTitle:Review`, routeIcon: 'check-circle'}
  ];
  protected $cart: Observable<CartDto>;
  protected $countries: Observable<Array<string>>;
  protected checkoutFormGroup: FormGroup<CheckoutForm>;
  protected steps: Array<StepInfoDto> = CheckoutComponent.Steps;

  constructor(private cartService: CartService,
              private orderService: OrderService) {
    this.$cart = cartService.$cart;
    this.$countries = orderService.getCountries();
    this.checkoutFormGroup = new FormGroup<CheckoutForm>({
      address: new FormGroup<CheckoutAddressForm>({
        city: new FormControl<string | null>(null, Validators.required),
        country: new FormControl<string | null>('', Validators.required),
        state: new FormControl<string | null>(null, Validators.required),
        street: new FormControl<string | null>(null, Validators.required),
        zipCode: new FormControl<string | null>(null, Validators.required)
      }),
      customer: new FormGroup<CheckoutCustomerForm>({
        eMail: new FormControl<string | null>(null, Validators.compose([Validators.required, Validators.email])),
        firstName: new FormControl<string | null>(null, Validators.required),
        lastName: new FormControl<string | null>(null, Validators.required),
        telephoneNumber: new FormControl<string | null>(null)
      })
    });
    this.checkoutFormGroup.setValue(this.convertToForm(this.orderService.customerData));
  }

  isInvalid(control: AbstractControl) {
    return control.invalid && control.touched;
  }

  saveCustomerData(): void {
    this.orderService.customerData = this.convertFromForm();
  }

  get firstNameControl() { return this.checkoutFormGroup.get('customer.firstName')!; }
  get lastNameControl() { return this.checkoutFormGroup.get('customer.lastName')!; }
  get eMailControl() { return this.checkoutFormGroup.get('customer.eMail')!; }
  get telephoneNumberControl() { return this.checkoutFormGroup.get('customer.telephoneNumber')!; }
  get countryControl() { return this.checkoutFormGroup.get('address.country')!; }
  get stateControl() { return this.checkoutFormGroup.get('address.state')!; }
  get zipCodeControl() { return this.checkoutFormGroup.get('address.zipCode')!; }
  get cityControl() { return this.checkoutFormGroup.get('address.city')!; }
  get streetControl() { return this.checkoutFormGroup.get('address.street')!; }

  private convertToForm(data: CustomerWithAddressDto | undefined) {
    return {
      customer: {
        telephoneNumber: data?.customer?.telephoneNumber ?? null,
        lastName: data?.customer?.lastName ?? null,
        firstName: data?.customer?.firstName ?? null,
        eMail: data?.customer?.eMail ?? null,
      },
      address: {
        zipCode: data?.address?.zipCode ?? null,
        state: data?.address?.state ?? null,
        country: data?.address?.country ?? null,
        street: data?.address?.street ?? null,
        city: data?.address?.city ?? null,
      }
    }
  }

  private convertFromForm(): CustomerWithAddressDto {
    return {
      ...this.orderService.customerData,
      customer: {
        creditCardNumber: null,
        ...this.checkoutFormGroup.get('customer')?.value
      },
      address: {
        ...this.checkoutFormGroup.get('address')?.value
      }
    }
  }
}

interface CheckoutForm {
  customer: FormGroup<CheckoutCustomerForm>,
  address: FormGroup<CheckoutAddressForm>
}

interface CheckoutCustomerForm {
  firstName: FormControl<string|null>,
  lastName: FormControl<string|null>,
  eMail: FormControl<string|null>,
  telephoneNumber: FormControl<string|null>
}

interface CheckoutAddressForm {
  street: FormControl<string|null>,
  city: FormControl<string|null>,
  state: FormControl<string|null>,
  country: FormControl<string|null>,
  zipCode: FormControl<string|null>
}
