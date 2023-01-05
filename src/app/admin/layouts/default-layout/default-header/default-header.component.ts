import { Component, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { ClassToggleService, HeaderComponent } from '@coreui/angular';
import {OAuthAuthenticationService} from "../../../modules/management/shared/o-auth-authentication.service";
import {AuthenticationApi} from "../../../shared/authentication.api";

@Component({
  selector: 'app-default-header',
  templateUrl: './default-header.component.html',
})
export class DefaultHeaderComponent extends HeaderComponent {

  @Input() sidebarId: string = "sidebar";

  public newMessages = new Array(4)
  public newTasks = new Array(5)
  public newNotifications = new Array(5)

  constructor(private classToggler: ClassToggleService,
              private authenticationService: AuthenticationApi) {
    super();
  }

  get avatarSrc(): string {
    return this.authenticationService.gravatarURL(128) ?? '';
  }
}
