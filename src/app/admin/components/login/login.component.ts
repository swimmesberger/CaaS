import { Component } from '@angular/core';
import {AuthenticationService} from "../../shared/authentication.service";
import {authConfig} from "../../auth.config";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-login-component',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  constructor(private router: Router,
              private activatedRoute: ActivatedRoute,
              private authenticationService: AuthenticationService) {
    authenticationService.loadConfig(authConfig).then(_ => {
      if(!authenticationService.isLoggedIn()) {
        authenticationService.login();
      } else {
        // noinspection JSIgnoredPromiseFromCall
        router.navigate(['../'], { relativeTo: activatedRoute })
      }
    })
  }
}
