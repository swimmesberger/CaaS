import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CartWidgetComponent } from './cart-widget.component';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {RouterTestingModule} from "@angular/router/testing";

describe('CartWidgetComponent', () => {
  let component: CartWidgetComponent;
  let fixture: ComponentFixture<CartWidgetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CartWidgetComponent ],
      imports: [ HttpClientTestingModule, RouterTestingModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CartWidgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
