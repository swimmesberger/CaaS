import { Component, OnInit } from '@angular/core';
import {Router} from "@angular/router";
import {CartWithAddressDto} from "../../shared/order/models/cartWithAddressDto";

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss']
})
export class PaymentComponent implements OnInit {
  cartState: CartWithAddressDto;

  constructor(private router: Router) {
    const cartState: CartWithAddressDto = router.getCurrentNavigation()?.extras.state?.['cartData'];
    if (!cartState) {
      // noinspection JSIgnoredPromiseFromCall
      router.navigate(['/cart']);
    }
    this.cartState = cartState;
  }

  ngOnInit(): void {
  }

}
