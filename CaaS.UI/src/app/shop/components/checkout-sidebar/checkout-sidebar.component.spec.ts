import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutSidebarComponent } from './checkout-sidebar.component';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {PromoInputComponent} from "../promo-input/promo-input.component";
import {ReactiveFormsModule} from "@angular/forms";

describe('CheckoutSidebarComponent', () => {
  let component: CheckoutSidebarComponent;
  let fixture: ComponentFixture<CheckoutSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckoutSidebarComponent, PromoInputComponent ],
      imports: [ HttpClientTestingModule, ReactiveFormsModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckoutSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
