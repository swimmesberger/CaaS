import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutCompleteComponent } from './checkout-complete.component';
import {RouterTestingModule} from "@angular/router/testing";

describe('OrderCompletePageComponent', () => {
  let component: CheckoutCompleteComponent;
  let fixture: ComponentFixture<CheckoutCompleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckoutCompleteComponent ],
      imports: [ RouterTestingModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckoutCompleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
