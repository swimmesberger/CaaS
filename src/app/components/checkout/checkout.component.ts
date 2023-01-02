import {Component, ElementRef, OnDestroy, OnInit, QueryList, ViewChildren} from '@angular/core';
import {CartService} from "../../shared/cart/cart.service";
import {Observable, Subscription} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {FormArray, FormControl, FormGroup} from "@angular/forms";

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
  host: {
    class: 'container pb-5 mb-2 mb-md-4'
  }
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private cartSub: Subscription | undefined;
  @ViewChildren("cartItemQuantity") cartItemQuantities!: QueryList<ElementRef>;
  protected $cart: Observable<CartDto>;
  protected cartFormGroup: FormGroup<CartForm>;
  protected cartItemsFormArray: FormArray<FormGroup<CartItemForm>>;
  protected isCartRefreshing: boolean = false;

  constructor(private cartService: CartService) {
    this.$cart = cartService.$cart;
    this.cartItemsFormArray = new FormArray<FormGroup<CartItemForm>>([]);
    this.cartFormGroup = new FormGroup<CartForm>({
      id: new FormControl<string | null>(null),
      totalPrice: new FormControl<number | null>(null),
      items: this.cartItemsFormArray
    })
  }

  ngOnInit(): void {
    this.cartSub = this.$cart.subscribe(this.onCartChanged.bind(this));
  }

  ngOnDestroy() {
    this.cartSub?.unsubscribe();
  }

  onCartChanged(cart: CartDto): void {
    this.cartItemsFormArray.clear();
    for (const cartItem of cart.items!) {
      this.cartItemsFormArray.push(this.createCartItemForm());
    }
    // @ts-ignore
    this.cartFormGroup.patchValue(cart!);
  }

  removeCartItem(e: MouseEvent, cartItemId: string | null | undefined): void {
    if (!cartItemId) return;
    const cartItemIdx = this.cartItemsFormArray.controls.findIndex(c => c.controls.id.value === cartItemId);
    if(cartItemIdx < 0) return;
    this.cartItemsFormArray.removeAt(cartItemIdx);
  }

  async updateCart(e: MouseEvent): Promise<void> {
    this.isCartRefreshing = true;
    try {
      // @ts-ignore
      const cartDto: CartDto = {...this.cartFormGroup.getRawValue()};
      await this.cartService.updateCart(cartDto);
    } finally {
      this.isCartRefreshing = false;
    }
  }

  private createCartItemForm(): FormGroup<CartItemForm> {
    return new FormGroup<CartItemForm>({
      id: new FormControl<string | null>(null),
      product: new FormGroup<CartItemProductForm>({
        id: new FormControl<string | null>(null),
        price: new FormControl<number | null>(null),
        name: new FormControl<string | null>(null),
        imageSrc: new FormControl<string | null>(null)
      }),
      amount: new FormControl<number | null>(null)
    });
  }
}

interface CartItemForm {
  id: FormControl<string|null>,
  product: FormGroup<CartItemProductForm>;
  amount: FormControl<number|null>;
}

interface CartItemProductForm {
  id: FormControl<string|null>,
  name: FormControl<string|null>,
  price: FormControl<number|null>,
  imageSrc: FormControl<string|null>,
}

interface CartForm {
  id: FormControl<string|null>,
  items: FormArray<FormGroup<CartItemForm>>,
  totalPrice: FormControl<number|null>,
}

