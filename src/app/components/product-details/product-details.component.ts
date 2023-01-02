import { Component } from '@angular/core';
import {switchMap, map, Observable} from "rxjs";
import {ProductStoreService} from "../../shared/product/product-store.service";
import {ProductDetailDto} from "../../shared/product/models/productDetailDto";
import {ActivatedRoute} from "@angular/router";
import {ProductGalleryItemDto} from "../product-gallery/product-gallery-item-dto";

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent {
  protected $product: Observable<ProductWithAdditions>;

  constructor(private route: ActivatedRoute, private productStoreService: ProductStoreService) {
    this.$product = this.route.paramMap.pipe(switchMap(params => {
      return this.productStoreService.getProductById(params.get('id')!);
    }), map(product => {
      return new ProductWithAdditions(product, this.getGalleryItems(product), 74);
    }));
  }

  // backend does not support multiple images currently, therefore we simulate more images via robohash
  getGalleryItems(product: ProductDetailDto): Array<ProductGalleryItemDto> {
    let galleryItems: Array<ProductGalleryItemDto> = [];
    if (product.imageSrc) {
      galleryItems.push({
        thImageSrc: product.imageSrc,
        imageSrc: product.imageSrc,
        imageAlt: product.name!
      })
    }
    let idx = 0;
    while(galleryItems.length < 4) {
      let suffix = idx <= 0 ? '' : '-' + idx;
      galleryItems.push({
        thImageSrc: `https://robohash.org/${product.id}${suffix}.png?size=156x156`,
        imageSrc: `https://robohash.org/${product.id}${suffix}.png?size=471x471`,
        imageAlt: product.name!
      });
      idx += 1;
    }
    return galleryItems;
  }
}

class ProductWithAdditions {
  constructor(
    public details: ProductDetailDto,
    public galleryItems: Array<ProductGalleryItemDto>,
    public numOfReviews: number
  ) {}
}
