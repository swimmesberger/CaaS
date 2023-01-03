import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutReviewPageComponent } from './checkout-review-page.component';

describe('CheckoutReviewPageComponent', () => {
  let component: CheckoutReviewPageComponent;
  let fixture: ComponentFixture<CheckoutReviewPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckoutReviewPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckoutReviewPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
