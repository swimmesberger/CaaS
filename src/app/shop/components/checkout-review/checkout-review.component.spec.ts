import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutReviewComponent } from './checkout-review.component';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {RouterTestingModule} from "@angular/router/testing";
import {CheckoutStepsComponent} from "../checkout-steps/checkout-steps.component";
import {CheckoutSidebarComponent} from "../checkout-sidebar/checkout-sidebar.component";
import {PromoInputComponent} from "../promo-input/promo-input.component";
import {ReactiveFormsModule} from "@angular/forms";

describe('CheckoutReviewComponent', () => {
  let component: CheckoutReviewComponent;
  let fixture: ComponentFixture<CheckoutReviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ RouterTestingModule, HttpClientTestingModule, ReactiveFormsModule ],
      declarations: [ CheckoutReviewComponent, CheckoutStepsComponent, CheckoutSidebarComponent, PromoInputComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckoutReviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
