import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppKeyLoginComponent } from './app-key-login.component';

describe('AppKeyLoginComponent', () => {
  let component: AppKeyLoginComponent;
  let fixture: ComponentFixture<AppKeyLoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AppKeyLoginComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppKeyLoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
