import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShopEditComponent } from './shop-edit.component';

describe('ShopAddComponent', () => {
  let component: ShopEditComponent;
  let fixture: ComponentFixture<ShopEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShopEditComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShopEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
