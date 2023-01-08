import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OAuthLoginComponent } from './o-auth-login.component';
import {RouterTestingModule} from "@angular/router/testing";
import {OAuthModule} from "angular-oauth2-oidc";
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('LoginComponentComponent', () => {
  let component: OAuthLoginComponent;
  let fixture: ComponentFixture<OAuthLoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OAuthLoginComponent ],
      imports: [ RouterTestingModule, HttpClientTestingModule, OAuthModule.forRoot() ]
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
