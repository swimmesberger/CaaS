import { Component } from '@angular/core';
import { environment } from '../../environments/environment';

@Component({
  selector: 'shop-root',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent {
  environment = environment;
}
