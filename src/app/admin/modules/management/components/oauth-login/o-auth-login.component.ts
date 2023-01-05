import { Component } from '@angular/core';
import {OAuthAuthenticationService} from "../../shared/o-auth-authentication.service";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-login-component',
  templateUrl: './o-auth-login.component.html',
  styleUrls: ['./o-auth-login.component.scss']
})
export class OAuthLoginComponent {
  constructor(private router: Router,
              private activatedRoute: ActivatedRoute,
              private authenticationService: OAuthAuthenticationService) {
    authenticationService.loadConfig().then(_ => {
      if(!authenticationService.isLoggedIn()) {
        authenticationService.login();
      } else {
        // noinspection JSIgnoredPromiseFromCall
        router.navigate(['../'], { relativeTo: activatedRoute })
      }
    })
  }
}
