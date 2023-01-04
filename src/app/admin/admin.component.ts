import {Component, ViewEncapsulation} from '@angular/core';
import { iconSubset } from './icons/icon-subset';
import {IconSetService} from "@coreui/icons-angular";

@Component({
  selector: 'app-admin',
  template: '<router-outlet></router-outlet>',
  styleUrls: ['admin.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AdminComponent {
  constructor(private iconSetService: IconSetService) {
    // iconSet singleton
    iconSetService.icons = { ...iconSubset };
  }
}
