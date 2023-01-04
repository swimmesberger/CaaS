import { Injectable } from '@angular/core';
import {catchError, lastValueFrom, Observable, throwError} from "rxjs";
import {OrderStoreService} from "./order-store.service";
import {CustomerWithAddressDto} from "./models/customerWithAddressDto";
import {CartService} from "../cart/cart.service";
import {OrderDto} from "./models/orderDto";
import { v4 as uuidv4 } from 'uuid';
import {HttpErrorResponse} from "@angular/common/http";
import {ProblemDetailsDto} from "../problemDetailsDto";
import {CaasPaymentError} from "../errors/caasPaymentError";

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private static readonly CustomerDataKey = "customerData";

  private _customerData?: CustomerWithAddressDto;

  constructor(private orderStoreService: OrderStoreService,
              private cartService: CartService) { }

  public async createOrder(billingData: CustomerWithAddressDto): Promise<OrderDto> {
    if (!this.cartService.cardId || !billingData.address) {
      throw new Error("Invalid state");
    }
    if (billingData.customer && !billingData.customer.id) {
      billingData.customer.id = uuidv4();
    }
    const order = await this.addOrder(billingData);
    this.cartService.resetCart()
    localStorage.removeItem(OrderService.CustomerDataKey);
    return order;
  }

  public getCountries(): Observable<Array<string>> {
    return this.orderStoreService.getCountries();
  }

  set customerData(data: CustomerWithAddressDto | undefined) {
    if (data?.creditCard) {
      data = {
        ...data,
        customer: {
          ...data?.customer,
          creditCardNumber: data.creditCard.creditCardNumber
        }
      }
    }
    this._customerData = data;
    localStorage.setItem(OrderService.CustomerDataKey, JSON.stringify(this._customerData));
  }

  get customerData(): CustomerWithAddressDto | undefined {
    if (!this._customerData) {
      const customerDataJson = localStorage.getItem(OrderService.CustomerDataKey);
      if (customerDataJson) {
        this._customerData = JSON.parse(customerDataJson);
      }
    }
    return this._customerData;
  }

  private async addOrder(billingData: CustomerWithAddressDto): Promise<OrderDto> {
    // retry loop for conflict
    let order: OrderDto;
    let retryCount = 0;
    while (true) {
      try {
        order = await lastValueFrom(this.orderStoreService
          .createOrder(this.cartService.cardId!, billingData.address!, billingData.customer!)
          .pipe(catchError(this.handleCreateOrderError.bind(this)))
        );
        break;
      } catch (e: any) {
        if (retryCount < 20 && e.status === 409) {
          retryCount += 1;
          continue;
        }
        throw e;
      }
    }
    return order;
  }

  private handleCreateOrderError(error: any) : Observable<any> {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 422) {
        const problemDetails : ProblemDetailsDto = error.error;
        if (problemDetails.type?.startsWith('payment_')) {
          throw new CaasPaymentError(problemDetails.title ?? '');
        }
      }
    }

    return throwError(() => error);
  }
}
