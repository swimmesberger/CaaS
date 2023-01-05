import {Component, ViewEncapsulation} from '@angular/core';
import {environment} from '../../environments/environment';

@Component({
  selector: 'shop-root',
  template: `
    <main class="page-wrapper">
      <router-outlet></router-outlet>
    </main>
  `,
  styleUrls: ['./styles.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ShopComponent {
  environment = environment;
}
