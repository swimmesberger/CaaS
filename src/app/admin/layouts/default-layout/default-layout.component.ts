import { Component } from '@angular/core';

import {NavProviderApi} from "../../shared/nav-provider.api";
import {INavData} from "@coreui/angular";

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
})
export class DefaultLayoutComponent {

  public navItems: Array<INavData> = [];

  public perfectScrollbarConfig = {
    suppressScrollX: true,
  };

  constructor(private navProvider: NavProviderApi) {
    navProvider.navItems().then(items => {
      this.navItems = items;
    });
  }
}
