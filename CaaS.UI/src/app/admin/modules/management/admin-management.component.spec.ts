import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminManagementComponent } from './admin-management.component';
import {RouterTestingModule} from "@angular/router/testing";

describe('AdminManagementComponent', () => {
  let component: AdminManagementComponent;
  let fixture: ComponentFixture<AdminManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminManagementComponent ],
      imports: [RouterTestingModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
