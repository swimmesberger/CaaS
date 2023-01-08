import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageNavbarLayoutComponent } from './page-navbar-layout.component';
import {RouterTestingModule} from "@angular/router/testing";
import {NavbarComponent} from "../../components/navbar/navbar.component";
import {ProductSearchComponent} from "../../components/product-search/product-search.component";
import {CartWidgetComponent} from "../../components/cart-widget/cart-widget.component";
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('PageNavbarLayoutComponent', () => {
  let component: PageNavbarLayoutComponent;
  let fixture: ComponentFixture<PageNavbarLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PageNavbarLayoutComponent, NavbarComponent, ProductSearchComponent, CartWidgetComponent ],
      imports: [ RouterTestingModule, HttpClientTestingModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PageNavbarLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
