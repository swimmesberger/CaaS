import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageHeaderLayoutComponent } from './page-header-layout.component';

describe('PageHeaderLayoutComponent', () => {
  let component: PageHeaderLayoutComponent;
  let fixture: ComponentFixture<PageHeaderLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PageHeaderLayoutComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PageHeaderLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
