import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OAuthLoginComponent } from './o-auth-login.component';

describe('LoginComponentComponent', () => {
  let component: OAuthLoginComponent;
  let fixture: ComponentFixture<OAuthLoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OAuthLoginComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OAuthLoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
