import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminShopComponent } from './admin-shop.component';

describe('AdminComponent', () => {
  let component: AdminShopComponent;
  let fixture: ComponentFixture<AdminShopComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminShopComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminShopComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
