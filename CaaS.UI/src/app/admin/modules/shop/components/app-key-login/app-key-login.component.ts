import { Component } from '@angular/core';
import {AppKeyAuthenticationService} from "../../shared/app-key-authentication.service";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-app-key-login',
  templateUrl: './app-key-login.component.html',
  styleUrls: ['./app-key-login.component.scss']
})
export class AppKeyLoginComponent  {
  private _redirectUri: string;
  form: any = {
    email: null,
    appKey: null
  };
  isLoginFailed = false;
  isLoading = false;

  constructor(private activatedRoute: ActivatedRoute,
              private router: Router,
              private authenticationService: AppKeyAuthenticationService) {
    this._redirectUri = activatedRoute.snapshot.queryParams['redirectUri'];
  }

  async login(): Promise<void> {
    this.isLoading = true;
    const { email, appKey } = this.form;
    try {
      this.isLoginFailed = false;
      const success = await this.authenticationService.login(email, appKey, this._redirectUri);
      if(!success) {
        this.isLoginFailed = true;
      }
    }finally {
      this.isLoading = false;
    }
  }

}
