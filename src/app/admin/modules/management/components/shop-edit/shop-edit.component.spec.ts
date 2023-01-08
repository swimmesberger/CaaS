import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShopEditComponent } from './shop-edit.component';
import {RouterTestingModule} from "@angular/router/testing";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {ReactiveFormsModule} from "@angular/forms";

describe('ShopAddComponent', () => {
  let component: ShopEditComponent;
  let fixture: ComponentFixture<ShopEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShopEditComponent ],
      imports: [ RouterTestingModule, HttpClientTestingModule, ReactiveFormsModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShopEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
