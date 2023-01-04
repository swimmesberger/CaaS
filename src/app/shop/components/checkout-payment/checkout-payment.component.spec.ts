import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutPaymentComponent } from './checkout-payment.component';

describe('PaymentComponent', () => {
  let component: CheckoutPaymentComponent;
  let fixture: ComponentFixture<CheckoutPaymentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckoutPaymentComponent ]
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
