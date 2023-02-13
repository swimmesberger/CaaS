import { Component, Input } from '@angular/core';

import { ClassToggleService, HeaderComponent } from '@coreui/angular';
import {AuthenticationApi} from "../../../shared/authentication.api";

@Component({
  selector: 'app-default-header',
  templateUrl: './default-header.component.html',
})
export class DefaultHeaderComponent extends HeaderComponent {
  constructor(private authenticationService: AuthenticationApi) {
    super();
  }

  logout(e: Event): void {
    e.stopPropagation();
    e.preventDefault();
    this.authenticationService.logout();
  }

  get avatarSrc(): string {
    return this.authenticationService.gravatarURL(128) ?? '';
  }
}
