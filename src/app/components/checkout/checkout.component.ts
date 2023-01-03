import { Component, OnInit } from '@angular/core';
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";
import {OrderStoreService} from "../../shared/order/order-store.service";
import {CustomerDto} from "../../shared/cart/models/customerDto";
import {AddressDto} from "../../shared/order/models/addressDto";
import {CartWithAddressDto} from "../../shared/order/models/cartWithAddressDto";

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
  protected $countries: Observable<Array<string>>;
  protected checkoutFormGroup: FormGroup<CheckoutForm>;

  constructor(private cartService: CartService,
              private orderService: OrderStoreService) {
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
  }

  ngOnInit(): void {
  }

  isInvalid(control: AbstractControl) {
    return control.invalid;
  }

  getCartWithCustomerData(cart: CartDto): CartWithAddressDto {
    const customer: CustomerDto = {
      ...this.checkoutFormGroup.get('customer')?.value
    }
    const address: AddressDto = {
      ...this.checkoutFormGroup.get('address')?.value
    }
    return {
      cart: {
        customer: customer,
        ...cart
      },
      address: address
    }
  }

  get firstNameControl() { return this.checkoutFormGroup.get('customer.firstName')!; }
  get lastNameControl() { return this.checkoutFormGroup.get('customer.lastName')!; }
  get eMailControl() { return this.checkoutFormGroup.get('customer.eMail')!; }
  get telephoneNumberControl() { return this.checkoutFormGroup.get('customer.telephoneNumber')!; }
  get countryControl() { return this.checkoutFormGroup.get('address.country')!; }
  get zipCodeControl() { return this.checkoutFormGroup.get('address.zipCode')!; }
  get cityControl() { return this.checkoutFormGroup.get('address.city')!; }
  get streetControl() { return this.checkoutFormGroup.get('address.street')!; }
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
