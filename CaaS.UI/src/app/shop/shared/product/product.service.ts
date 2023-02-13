import { Injectable } from '@angular/core';
import {ProductMinimalDto} from "./models/productMinimalDto";
import {ProductStoreService} from "./product-store.service";
import {Observable} from "rxjs";
import {ProductDetailDto} from "./models/productDetailDto";
import {ProductGalleryItemDto} from "./models/productGalleryItemDto";

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private productStoreService: ProductStoreService) { }

  public getProductById(productId: string): Observable<ProductDetailDto> {
    return this.productStoreService.getProductById(productId);
  }


  // backend does not support multiple images currently, therefore we simulate more images via robohash
  public galleryItems(product: ProductDetailDto): Array<ProductGalleryItemDto> {
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
        thImageSrc: this.fallbackProductImage(product.id + suffix, 156, 156),
        imageSrc: this.fallbackProductImage(product.id + suffix, 471, 471),
        imageAlt: product.name!
      });
      idx += 1;
    }
    return galleryItems;
  }

  public productImage(product: ProductMinimalDto | undefined, width: number, height: number): string {
    return product?.imageSrc ?? this.fallbackProductImage(product?.id, width, height);
  }

  private fallbackProductImage(productId: string | undefined, width: number, height: number): string {
    return `https://robohash.org/${productId}.png?size=${width}x${height}`;
  }
}
