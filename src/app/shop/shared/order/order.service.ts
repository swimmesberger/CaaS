import { Injectable } from '@angular/core';
import {BehaviorSubject, catchError, lastValueFrom, map, Observable, throwError} from "rxjs";
import {OrderStoreService} from "./order-store.service";
import {CustomerWithAddressDto} from "./models/customerWithAddressDto";
import {CartService} from "../cart/cart.service";
import {OrderDto} from "./models/orderDto";
import { v4 as uuidv4 } from 'uuid';
import {HttpErrorResponse} from "@angular/common/http";
import {ProblemDetailsDto} from "../problemDetailsDto";
import {CaasPaymentError} from "../errors/caasPaymentError";
import {TenantIdService} from "../shop/tenant-id.service";
import {CaasDuplicateCustomerEmailError} from "../errors/caasDuplicateCustomerEmailError";

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private static readonly CustomerDataKey = "customerData";

  private readonly _$customerData: BehaviorSubject<CustomerWithAddressDto | null>;
  private readonly _$customerDataOb: Observable<CustomerWithAddressDto | null>;

  constructor(private orderStoreService: OrderStoreService,
              private cartService: CartService,
              private tenantIdService: TenantIdService) {
    this._$customerData = new BehaviorSubject<CustomerWithAddressDto | null>(null);
    this.tenantIdService.$tenantId.pipe(map(tenantId => {
      if (tenantId == null) return null;
      const customerDataJson = localStorage.getItem(this.getCustomerDataKey(tenantId));
      if (customerDataJson) {
        return JSON.parse(customerDataJson);
      }
      return null;
    })).subscribe(this._$customerData);
    this._$customerDataOb = this._$customerData.asObservable();
  }

  public async createOrder(billingData: CustomerWithAddressDto): Promise<OrderDto> {
    if (!this.cartService.cardId || !billingData.address) {
      throw new Error("Invalid state");
    }
    if (billingData.customer && !billingData.customer.id) {
      billingData.customer.id = uuidv4();
    }
    const order = await this.addOrder(billingData);
    this.cartService.resetCart()
    localStorage.removeItem(this.customerDataKey);
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
    localStorage.setItem(this.customerDataKey, JSON.stringify(data));
    this._$customerData.next(data ?? null);
  }

  get $customerData(): Observable<CustomerWithAddressDto | null> {
    return this._$customerDataOb;
  }

  get customerData(): CustomerWithAddressDto | undefined {
    const customerData = this._$customerData.value;
    return customerData ?? undefined;
  }

  private get customerDataKey() {
    return this.getCustomerDataKey(this.tenantIdService.tenantId);
  }

  private getCustomerDataKey(tenantId: string | null) {
    return `${tenantId}_${OrderService.CustomerDataKey}`
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
        if (problemDetails.type === 'customer_e_mail_key') {
          throw new CaasDuplicateCustomerEmailError(problemDetails.title ?? '');
        }
      }
    }

    return throwError(() => error);
  }
}
