import {Component, Input, OnInit} from '@angular/core';
import {WishlistButtonStyle} from "./wishlist-button-style";

@Component({
  selector: 'app-wishlist-button',
  templateUrl: './wishlist-button.component.html',
  styleUrls: ['./wishlist-button.component.scss'],
  host: {
    'class': 'btn-wishlist'
  }
})
export class WishlistButtonComponent implements OnInit {
  @Input() wishlistButtonStyle: string = WishlistButtonStyle.Small;

  constructor() { }

  ngOnInit(): void {
  }

}
