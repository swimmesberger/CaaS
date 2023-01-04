import {Component, ElementRef, HostListener, ViewChild} from '@angular/core';
import {TenantIdService} from "../../shared/shop/tenant-id.service";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  @ViewChild("navbarSticky") private navbar!: ElementRef;

  constructor(protected tenantService: TenantIdService) {
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
}
