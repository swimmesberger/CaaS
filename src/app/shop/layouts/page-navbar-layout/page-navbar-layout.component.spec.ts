import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageNavbarLayoutComponent } from './page-navbar-layout.component';

describe('PageNavbarLayoutComponent', () => {
  let component: PageNavbarLayoutComponent;
  let fixture: ComponentFixture<PageNavbarLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PageNavbarLayoutComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PageNavbarLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
