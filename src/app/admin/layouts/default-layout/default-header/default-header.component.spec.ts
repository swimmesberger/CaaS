import { ComponentFixture, TestBed } from '@angular/core/testing';

import {
  AvatarModule,
  BadgeModule,
  BreadcrumbModule,
  DropdownModule,
  GridModule,
  HeaderModule,
  NavModule, SidebarModule
} from '@coreui/angular';
import { IconSetService } from '@coreui/icons-angular';
import { iconSubset } from '../../../icons/icon-subset';
import { DefaultHeaderComponent } from './default-header.component';
import { RouterTestingModule } from '@angular/router/testing';
import {AuthenticationApi} from "../../../shared/authentication.api";
import {OAuthAuthenticationService} from "../../../modules/management/shared/o-auth-authentication.service";
import {AppKeyAuthenticationService} from "../../../modules/shop/shared/app-key-authentication.service";
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('DefaultHeaderComponent', () => {
  let component: DefaultHeaderComponent;
  let fixture: ComponentFixture<DefaultHeaderComponent>;
  let iconSetService: IconSetService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DefaultHeaderComponent],
      imports: [HttpClientTestingModule, GridModule, HeaderModule, NavModule, BadgeModule, AvatarModule, DropdownModule, BreadcrumbModule, RouterTestingModule, SidebarModule],
      providers: [
        IconSetService,
        { provide: AuthenticationApi, useClass: AppKeyAuthenticationService }
      ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    iconSetService = TestBed.inject(IconSetService);
    iconSetService.icons = { ...iconSubset };

    fixture = TestBed.createComponent(DefaultHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
