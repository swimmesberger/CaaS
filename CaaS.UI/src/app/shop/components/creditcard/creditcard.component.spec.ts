import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreditcardComponent } from './creditcard.component';
import {ReactiveFormsModule} from "@angular/forms";

describe('CreditcardComponent', () => {
  let component: CreditcardComponent;
  let fixture: ComponentFixture<CreditcardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreditcardComponent ],
      imports: [ReactiveFormsModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreditcardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});