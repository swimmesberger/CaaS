import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutComponent } from './checkout.component';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {CheckoutStepsComponent} from "../checkout-steps/checkout-steps.component";
import {CheckoutSidebarComponent} from "../checkout-sidebar/checkout-sidebar.component";
import {RouterTestingModule} from "@angular/router/testing";
import {ReactiveFormsModule} from "@angular/forms";
import {PromoInputComponent} from "../promo-input/promo-input.component";

describe('CheckoutComponent', () => {
  let component: CheckoutComponent;
  let fixture: ComponentFixture<CheckoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckoutComponent, CheckoutStepsComponent, CheckoutSidebarComponent, PromoInputComponent ],
      imports: [ HttpClientTestingModule, RouterTestingModule, ReactiveFormsModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
