import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PromoInputComponent } from './promo-input.component';

describe('PromoInputComponent', () => {
  let component: PromoInputComponent;
  let fixture: ComponentFixture<PromoInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PromoInputComponent ]
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
