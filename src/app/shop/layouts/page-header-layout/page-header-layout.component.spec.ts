import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageHeaderLayoutComponent } from './page-header-layout.component';
import {NavbarComponent} from "../../components/navbar/navbar.component";
import {RouterTestingModule} from "@angular/router/testing";
import {PageHeaderComponent} from "../../components/page-header/page-header.component";
import {ProductSearchComponent} from "../../components/product-search/product-search.component";
import {CartWidgetComponent} from "../../components/cart-widget/cart-widget.component";
import {BreadcrumbComponent} from "../../components/breadcrumb/breadcrumb.component";
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('PageHeaderLayoutComponent', () => {
  let component: PageHeaderLayoutComponent;
  let fixture: ComponentFixture<PageHeaderLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PageHeaderLayoutComponent, NavbarComponent, PageHeaderComponent, ProductSearchComponent, CartWidgetComponent, BreadcrumbComponent ],
      imports: [ RouterTestingModule, HttpClientTestingModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PageHeaderLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
