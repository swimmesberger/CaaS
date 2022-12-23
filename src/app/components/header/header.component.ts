import {AfterViewInit, Component, HostListener, OnDestroy} from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements AfterViewInit, OnDestroy {
  navbar!: HTMLElement;

  ngAfterViewInit(): void {
    this.navbar = document.querySelector(".navbar-sticky")!;
  }

  ngOnDestroy() {

  }

  @HostListener('window:scroll', ['$event']) // for window scroll events
  onScroll() {
      if (window.scrollY > 120) {
        document.body.style.paddingTop = this.navbar.offsetHeight + "px";
        this.navbar.classList.add("navbar-stuck")
      } else {
        document.body.style.paddingTop = "";
        this.navbar.classList.remove("navbar-stuck")
      }
  }
}
