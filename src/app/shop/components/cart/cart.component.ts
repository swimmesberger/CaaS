import {Component, ElementRef, OnDestroy, OnInit, QueryList, ViewChildren} from '@angular/core';
import {CartService} from "../../shared/cart/cart.service";
import {Observable, Subscription} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";
import {FormArray, FormControl, FormGroup, Validators} from "@angular/forms";
import {ProductService} from "../../shared/product/product.service";

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
  host: {
    class: 'container pb-5 mb-2 mb-md-4'
  }
})
export class CartComponent implements OnInit, OnDestroy {
  private _cartSub: Subscription | undefined;
  @ViewChildren("cartItemQuantity") cartItemQuantities!: QueryList<ElementRef>;
  protected $cart: Observable<CartDto>;
  protected cartFormGroup: FormGroup<CartForm>;
  protected cartItemsFormArray: FormArray<FormGroup<CartItemForm>>;
  protected isLoading: boolean = false;


  constructor(private cartService: CartService,
              private productService: ProductService) {
    this.$cart = cartService.$cart;
    this.cartItemsFormArray = new FormArray<FormGroup<CartItemForm>>([]);
    this.cartFormGroup = new FormGroup<CartForm>({
      id: new FormControl<string | null>(null),
      totalPrice: new FormControl<number | null>(null),
      items: this.cartItemsFormArray
    });
  }

  ngOnInit(): void {
    this._cartSub = this.$cart.subscribe(this.onCartChanged.bind(this));
  }

  ngOnDestroy() {
    this._cartSub?.unsubscribe();
  }

  productImage(cartItem: FormGroup<CartItemForm>, width: number, height: number): string {
    const imageSrc = cartItem.get('product.imageSrc')?.value;
    let productId = cartItem.get('product.id')?.value;
    if (productId == null) {
      productId = undefined;
    }
    return this.productService.productImage({ id: productId, imageSrc: imageSrc }, width, height);
  }

  removeCartItem(e: MouseEvent, cartItemId: string | null | undefined): void {
    if (!cartItemId) return;
    const cartItemIdx = this.cartItemsFormArray.controls.findIndex(c => c.controls.id.value === cartItemId);
    if(cartItemIdx < 0) return;
    this.cartItemsFormArray.removeAt(cartItemIdx);
  }

  async updateCart(): Promise<void> {
    this.isLoading = true;
    try {
      // @ts-ignore
      const cartDto: CartDto = {...this.cartFormGroup.getRawValue()};
      await this.cartService.updateCart(cartDto);
    } finally {
      this.isLoading = false;
    }
  }

  get cartTotalPrice(): number {
    return this.cartFormGroup.get('totalPrice')?.value ?? 0;
  }

  private onCartChanged(cart: CartDto): void {
    this.cartItemsFormArray.clear();
    for (const cartItem of cart.items!) {
      this.cartItemsFormArray.push(this.createCartItemForm());
    }
    // @ts-ignore
    this.cartFormGroup.patchValue(cart!);
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
      amount: new FormControl<number | null>(null, Validators.compose([Validators.required, Validators.min(1)]))
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
