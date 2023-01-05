import {Component} from '@angular/core';
import {environment} from '../../environments/environment';

@Component({
  selector: 'shop-root',
  template: `
    <main class="page-wrapper">
      <router-outlet></router-outlet>
    </main>
  `
})
export class ShopComponent {
  environment = environment;
}
