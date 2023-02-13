import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutPaymentComponent } from './checkout-payment.component';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {CheckoutStepsComponent} from "../checkout-steps/checkout-steps.component";
import {CheckoutSidebarComponent} from "../checkout-sidebar/checkout-sidebar.component";
import {CreditcardComponent} from "../creditcard/creditcard.component";
import {RouterTestingModule} from "@angular/router/testing";
import {ReactiveFormsModule} from "@angular/forms";
import {PromoInputComponent} from "../promo-input/promo-input.component";

describe('PaymentComponent', () => {
  let component: CheckoutPaymentComponent;
  let fixture: ComponentFixture<CheckoutPaymentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ HttpClientTestingModule, RouterTestingModule, ReactiveFormsModule ],
      declarations: [ CheckoutPaymentComponent, CheckoutStepsComponent, CheckoutSidebarComponent, CreditcardComponent, PromoInputComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckoutPaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
