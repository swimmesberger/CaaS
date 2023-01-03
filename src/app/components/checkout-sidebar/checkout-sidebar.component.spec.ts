import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutSidebarComponent } from './checkout-sidebar.component';

describe('CheckoutSidebarComponent', () => {
  let component: CheckoutSidebarComponent;
  let fixture: ComponentFixture<CheckoutSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckoutSidebarComponent ]
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
