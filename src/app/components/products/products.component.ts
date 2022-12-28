import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import {ProductStoreService} from "../../shared/product-store.service";
import {bufferCount, concatMap, Observable, share, toArray} from "rxjs";
import {ProductMinimalDtoPagedResult} from "../../shared/models/productMinimalDtoPagedResult";
import {ProductMinimalDto} from "../../shared/models/productMinimalDto";

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {
  private static readonly ProductsPerRow = 4;

  private environment = environment;
  protected productsPerRow: Observable<Array<Array<ProductMinimalDto>>>;
  private productPage: Observable<ProductMinimalDtoPagedResult>;

  constructor(private productStoreService: ProductStoreService) {
    this.productPage = this.productStoreService.search().pipe(share());
    this.productsPerRow = this.productPage.pipe(concatMap(p => p.items!), bufferCount(ProductsComponent.ProductsPerRow), toArray());
  }

  ngOnInit(): void {
  }

}
