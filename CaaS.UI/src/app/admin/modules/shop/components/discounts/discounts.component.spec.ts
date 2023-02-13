import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscountsComponent } from './discounts.component';
import {RouterTestingModule} from "@angular/router/testing";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {CardModule, GridModule, TableModule} from "@coreui/angular";

describe('DiscountsComponent', () => {
  let component: DiscountsComponent;
  let fixture: ComponentFixture<DiscountsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DiscountsComponent ],
      imports: [ RouterTestingModule, HttpClientTestingModule, GridModule, CardModule, TableModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DiscountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
