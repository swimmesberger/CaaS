import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderCompletePageComponent } from './order-complete-page.component';

describe('OrderCompletePageComponent', () => {
  let component: OrderCompletePageComponent;
  let fixture: ComponentFixture<OrderCompletePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderCompletePageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderCompletePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
