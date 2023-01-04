import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutPaymentPageComponent } from './checkout-payment-page.component';

describe('PaymentPageComponent', () => {
  let component: CheckoutPaymentPageComponent;
  let fixture: ComponentFixture<CheckoutPaymentPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckoutPaymentPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckoutPaymentPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
