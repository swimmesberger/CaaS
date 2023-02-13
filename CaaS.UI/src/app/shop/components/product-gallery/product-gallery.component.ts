import {Component, Input, OnInit} from '@angular/core';
import {ProductGalleryItemDto} from "../../shared/product/models/productGalleryItemDto";

@Component({
  selector: 'app-product-gallery',
  templateUrl: './product-gallery.component.html',
  styleUrls: ['./product-gallery.component.scss'],
  host: {
    'class': 'product-gallery'
  }
})
export class ProductGalleryComponent implements OnInit {
  protected activeIdx: number = 0;
  @Input()
  public items: Array<ProductGalleryItemDto> = [];

  constructor() { }

  ngOnInit(): void {
  }

  onThumbnailClick(e: Event, clickedItem: ProductGalleryItemDto, clickedItemIdx: number): void {
    e.preventDefault();
    this.activeIdx = clickedItemIdx;
  }
}
