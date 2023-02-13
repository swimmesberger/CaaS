import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductSearchComponent } from './product-search.component';
import {RouterTestingModule} from "@angular/router/testing";

describe('ProductSearchComponent', () => {
  let component: ProductSearchComponent;
  let fixture: ComponentFixture<ProductSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProductSearchComponent ],
      imports: [ RouterTestingModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
