import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Observable, switchMap} from "rxjs";

@Component({
  selector: 'app-order-complete-page',
  templateUrl: './order-complete-page.component.html',
  styleUrls: ['./order-complete-page.component.scss']
})
export class OrderCompletePageComponent {
  protected $orderNumber: Observable<string>;

  constructor(private route: ActivatedRoute) {
    this.$orderNumber = this.route.queryParams.pipe(switchMap(params => {
      return <string>params['orderNumber']?.toString() ?? '';
    }));
  }
}
