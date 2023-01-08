import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShopsComponent } from './shops.component';
import {RouterTestingModule} from "@angular/router/testing";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {CardModule, GridModule, TableModule} from "@coreui/angular";

describe('ShopsComponent', () => {
  let component: ShopsComponent;
  let fixture: ComponentFixture<ShopsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ RouterTestingModule, HttpClientTestingModule, GridModule, CardModule, TableModule ],
      declarations: [ ShopsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShopsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
