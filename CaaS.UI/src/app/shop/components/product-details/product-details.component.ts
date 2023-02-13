import {Component, ElementRef, ViewChild} from '@angular/core';
import {switchMap, map, Observable} from "rxjs";
import {ProductDetailDto} from "../../shared/product/models/productDetailDto";
import {ActivatedRoute} from "@angular/router";
import {ProductGalleryItemDto} from "../../shared/product/models/productGalleryItemDto";
import {CartService} from "../../shared/cart/cart.service";
import {ProductService} from "../../shared/product/product.service";

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent {
  @ViewChild('quantitySelectElement') quantitySelectElement!: ElementRef;
  $product: Observable<ProductWithAdditions>;

  constructor(private route: ActivatedRoute,
              private productService: ProductService,
              private cartService: CartService) {
    this.$product = this.route.paramMap.pipe(switchMap(params => {
      return this.productService.getProductById(params.get('id')!);
    }), map(product => {
      return new ProductWithAdditions(product, this.getGalleryItems(product), 74);
    }));
  }

  getGalleryItems(product: ProductDetailDto): Array<ProductGalleryItemDto> {
    return this.productService.galleryItems(product);
  }

  async addProductToCart(e: Event, productId: string | undefined): Promise<void> {
    e.stopPropagation();
    e.preventDefault();
    if (!productId) return;
    if (!(this.quantitySelectElement.nativeElement instanceof HTMLSelectElement))return;
    const quantity: number = parseInt(this.quantitySelectElement.nativeElement.value, 10);
    await this.cartService.addProductToCart(productId, quantity);
  }
}

class ProductWithAdditions {
  constructor(
    public details: ProductDetailDto,
    public galleryItems: Array<ProductGalleryItemDto>,
    public numOfReviews: number
  ) {}
}
