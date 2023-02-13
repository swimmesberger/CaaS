import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PromoInputComponent } from './promo-input.component';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {ReactiveFormsModule} from "@angular/forms";

describe('PromoInputComponent', () => {
  let component: PromoInputComponent;
  let fixture: ComponentFixture<PromoInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PromoInputComponent ],
      imports: [ HttpClientTestingModule, ReactiveFormsModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PromoInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
