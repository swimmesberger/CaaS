import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscountEditComponent } from './discount-edit.component';
import {RouterTestingModule} from "@angular/router/testing";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {ReactiveFormsModule} from "@angular/forms";

describe('DiscountEditComponent', () => {
  let component: DiscountEditComponent;
  let fixture: ComponentFixture<DiscountEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DiscountEditComponent ],
      imports: [ RouterTestingModule, HttpClientTestingModule, ReactiveFormsModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DiscountEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
