import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminShopComponent } from './admin-shop.component';
import {RouterTestingModule} from "@angular/router/testing";

describe('AdminShopComponent', () => {
  let component: AdminShopComponent;
  let fixture: ComponentFixture<AdminShopComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminShopComponent ],
      imports: [ RouterTestingModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminShopComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
