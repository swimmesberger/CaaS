import {Component, ElementRef, HostListener, ViewChild} from '@angular/core';
import {CartService} from "../../shared/cart/cart.service";
import {Observable} from "rxjs";
import {CartDto} from "../../shared/cart/models/cartDto";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  @ViewChild("navbarSticky") private navbar!: ElementRef;
  protected $cart: Observable<CartDto>;

  constructor(private cartService: CartService) {
    this.$cart = cartService.$cart;
  }


  @HostListener('window:scroll', ['$event']) // for window scroll events
  onScroll() {
    const nativeNavbar = this.navbar.nativeElement;
    if (window.scrollY > 120 && document.documentElement.scrollHeight > 2000) {
      document.body.style.paddingTop = nativeNavbar.offsetHeight + "px";
      nativeNavbar.classList.add("navbar-stuck")
    } else {
      document.body.style.paddingTop = "";
      nativeNavbar.classList.remove("navbar-stuck")
    }
  }

  getProductCount(cart: CartDto): number {
    let sum = 0;
    for (let item of cart.items!) {
      sum += item?.amount ?? 0;
    }
    return sum;
  }
}
