import { Injectable } from '@angular/core';
import {
  BehaviorSubject,
  catchError, firstValueFrom, lastValueFrom,
  Observable,
  of,
  shareReplay,
  switchMap,
  throwError
} from "rxjs";
import {CartDto} from "./models/cartDto";
import {CartStoreService} from "./cart-store.service";
import { v4 as uuidv4 } from 'uuid';
import {CouponDto} from "./models/couponDto";
import {HttpErrorResponse} from "@angular/common/http";
import {ProblemDetailsDto} from "../problemDetailsDto";
import {CaasDuplicateCustomerEmailError} from "../errors/caasDuplicateCustomerEmailError";

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private static readonly CartIdKey = "cartId";

  private _$refreshCart: BehaviorSubject<void>;
  public $cart: Observable<CartDto>;

  constructor(private _cartStore: CartStoreService) {
    this._$refreshCart = new BehaviorSubject<void>(undefined);
    this.$cart = this._$refreshCart.pipe(
      switchMap(this.loadCart.bind(this)),
      shareReplay(1)
    )
  }

  public async addProductToCart(productId: string, productQuantity: number): Promise<void> {
    const curCart = await firstValueFrom(this.$cart);
    const productItemIdx = curCart.items?.findIndex(i => i.product?.id === productId);
    const cartItems = Array.from(curCart.items ?? []);
    if (productItemIdx === undefined || productItemIdx < 0) {
      cartItems.push({
        product: { id: productId },
        amount: productQuantity
      })
    } else {
      cartItems[productItemIdx] = {
        product: { id: productId },
        amount: curCart.items![productItemIdx].amount! + productQuantity
      }
    }
    await this.updateCart({
      ...curCart,
      items: cartItems
    });
  }

  public async setCouponOfCart(couponCode: string | null): Promise<void> {
    const curCart = await firstValueFrom(this.$cart);
    const coupons: Array<CouponDto> = [];
    if (couponCode) {
      coupons.push({code: couponCode});
    }
    await this.updateCart({
      ...curCart,
      coupons: coupons
    });
  }

  public resetCart(): void {
    localStorage.removeItem(CartService.CartIdKey);
    // refresh cart
    this._$refreshCart.next();
  }

  public async updateCart(cart: CartDto): Promise<void> {
    await lastValueFrom(this._cartStore.updateCart(cart).pipe(catchError(this.handleUpdateCartError.bind(this))));
    localStorage.setItem(CartService.CartIdKey, cart.id!);
    // refresh cart
    this._$refreshCart.next();
  }

  public get cardId(): string | null {
    return localStorage.getItem(CartService.CartIdKey);
  }

  private handleUpdateCartError(error: any) : Observable<any> {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 422) {
        const problemDetails : ProblemDetailsDto = error.error;
        if (problemDetails.type === 'customer_e_mail_key') {
          throw new CaasDuplicateCustomerEmailError(problemDetails.title ?? '');
        }
      }
    }

    return throwError(() => error);
  }

  private emptyCart(): CartDto {
    return {
      id: uuidv4(),
      totalPrice: 0,
      items: [],
      cartDiscounts: [],
      coupons: []
    };
  }

  private loadCart(): Observable<CartDto> {
    const cartId = localStorage.getItem(CartService.CartIdKey);
    if (!cartId) {
      return of<CartDto>(this.emptyCart())
    }
    return this._cartStore.getCartById(cartId).pipe(catchError(error => {
      if (error.status === 0) {
        console.error('An error occurred:', error.error);
      } else if(error.status === 404) {
        return of<CartDto>(this.emptyCart());
      } else {
        console.error(`Backend returned code ${error.status}, body was: `, error.error);
      }
      return throwError(() => new Error('Failed to load cart; please try again later.'));
    }));
  }
}
