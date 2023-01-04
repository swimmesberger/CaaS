import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Observable, of, switchMap} from "rxjs";

@Component({
  selector: 'app-checkout-complete',
  templateUrl: './checkout-complete.component.html',
  styleUrls: ['./checkout-complete.component.scss']
})
export class CheckoutCompleteComponent {
  protected $orderNumber: Observable<string>;

  constructor(private route: ActivatedRoute) {
    this.$orderNumber = this.route.queryParams.pipe(switchMap(params => {
      return of(params['orderNumber']?.toString() ?? '');
    }));
  }
}
