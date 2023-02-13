import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppKeyLoginComponent } from './app-key-login.component';
import {RouterTestingModule} from "@angular/router/testing";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {FormsModule} from "@angular/forms";
import {CardModule, FormModule, GridModule} from "@coreui/angular";

describe('AppKeyLoginComponent', () => {
  let component: AppKeyLoginComponent;
  let fixture: ComponentFixture<AppKeyLoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AppKeyLoginComponent ],
      imports: [ RouterTestingModule, FormsModule, HttpClientTestingModule, GridModule, CardModule, FormModule ]
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
