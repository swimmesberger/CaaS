import {Component, Input, OnInit} from '@angular/core';
import {ProductMinimalDto} from "../../shared/product/models/productMinimalDto";
import {CartService} from "../../shared/cart/cart.service";

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss']
})
export class ProductItemComponent implements OnInit {
  @Input() product: ProductMinimalDto = {};

  constructor(private cartService: CartService) { }

  ngOnInit(): void {
  }

  async addProductToCart(e: Event): Promise<void> {
    e.stopPropagation();
    e.preventDefault();
    e.stopImmediatePropagation()
    await this.cartService.addProductToCart(this.product?.id!, 1);
  }
}
