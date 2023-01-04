import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Observable, of, switchMap} from "rxjs";

@Component({
  selector: 'app-order-complete-page',
  templateUrl: './checkout-complete-page.component.html',
  styleUrls: ['./checkout-complete-page.component.scss']
})
export class CheckoutCompletePageComponent {
  protected $orderNumber: Observable<string>;

  constructor(private route: ActivatedRoute) {
    this.$orderNumber = this.route.queryParams.pipe(switchMap(params => {
      return of(params['orderNumber']?.toString() ?? '');
    }));
  }
}
