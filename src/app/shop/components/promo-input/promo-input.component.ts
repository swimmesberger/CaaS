import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {Observable, Subscription} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {CartService} from "../../shared/cart/cart.service";

@Component({
  selector: 'app-promo-input',
  templateUrl: './promo-input.component.html',
  styleUrls: ['./promo-input.component.scss']
})
export class PromoInputComponent implements OnInit, OnDestroy {
  private _cartSub: Subscription | undefined;
  protected $cart: Observable<CartDto>;
  protected couponFormGroup: FormGroup<CartCouponForm>;
  protected hasValidPromoCode: boolean = false;
  protected isLoading: boolean = false;

  constructor(private cartService: CartService) {
    this.$cart = cartService.$cart;
    this.couponFormGroup = new FormGroup<CartCouponForm>({
      promoCode: new FormControl<string | null>(null, Validators.required)
    });
  }

  ngOnInit(): void {
    this._cartSub = this.$cart.subscribe(this.onCartChanged.bind(this));
  }

  ngOnDestroy() {
    this._cartSub?.unsubscribe();
  }

  get promoCodeControl() { return this.couponFormGroup.get('promoCode')!; }

  get promoCode() { return this.promoCodeControl.value; }

  async applyCoupon(): Promise<void> {
    if (!this.promoCodeControl.valid) {
      return;
    }
    this.isLoading = true;
    try {
      const promoCode = this.promoCodeControl?.value;
      if (!promoCode) {
        return;
      }
      await this.cartService.setCouponOfCart(promoCode);
    } catch(HttpErrorResponse) {
      this.promoCodeControl.setErrors({'invalid-code': true});
    } finally {
      this.isLoading = false;
    }
  }

  async removeCoupon(e: Event): Promise<void> {
    e.stopPropagation();
    e.preventDefault();

    this.isLoading = true;
    try {
      await this.cartService.setCouponOfCart(null);
      this.promoCodeControl.reset()
    } finally {
      this.isLoading = false;
    }
  }

  private onCartChanged(cart: CartDto): void {
    if (cart.coupons && cart.coupons.length > 0) {
      this.promoCodeControl.setValue(cart.coupons[0].code)
      this.hasValidPromoCode = true;
    } else {
      this.hasValidPromoCode = false;
    }
  }
}


interface CartCouponForm {
  promoCode: FormControl<string|null>
}
