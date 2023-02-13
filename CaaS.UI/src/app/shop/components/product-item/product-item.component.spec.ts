import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductItemComponent } from './product-item.component';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {WishlistButtonComponent} from "../wishlist-button/wishlist-button.component";
import {RatingBarComponent} from "../rating-bar/rating-bar.component";
import {RouterTestingModule} from "@angular/router/testing";

describe('ProductItemComponent', () => {
  let component: ProductItemComponent;
  let fixture: ComponentFixture<ProductItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProductItemComponent, WishlistButtonComponent, RatingBarComponent ],
      imports: [ HttpClientTestingModule, RouterTestingModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
