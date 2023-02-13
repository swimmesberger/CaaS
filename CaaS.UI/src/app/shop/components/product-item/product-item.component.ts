import {Component, Input, OnInit} from '@angular/core';
import {ProductMinimalDto} from "../../shared/product/models/productMinimalDto";
import {CartService} from "../../shared/cart/cart.service";
import {ProductService} from "../../shared/product/product.service";

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss']
})
export class ProductItemComponent {
  @Input() product: ProductMinimalDto = {};

  constructor(private cartService: CartService,
              private productService: ProductService) { }

  async addProductToCart(e: Event): Promise<void> {
    e.stopPropagation();
    e.preventDefault();
    await this.cartService.addProductToCart(this.product?.id!, 1);
  }

  productImage(width: number, height: number): string {
    return this.productService.productImage(this.product, width, height);
  }
}
