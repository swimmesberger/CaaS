import { Component } from '@angular/core';
import {ProductStoreService} from "../../shared/product/product-store.service";
import {Observable, switchMap} from "rxjs";
import {ProductMinimalDtoPagedResult} from "../../shared/product/models/productMinimalDtoPagedResult";
import {ProductMinimalDto} from "../../shared/product/models/productMinimalDto";
import {ActivatedRoute} from "@angular/router";
import {KeysetPaginationDirection} from "../../shared/product/models/keysetPaginationDirection";
import {ParsedPaginationToken} from "../../shared/product/models/parsedPaginationToken";

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent {
  private static readonly ProductsPerRow = 4;
  protected $productPage: Observable<ProductMinimalDtoPagedResult>;

  constructor(
    private route: ActivatedRoute,
    private productStoreService: ProductStoreService) {
    this.$productPage = this.route.queryParams.pipe(switchMap(params => {
      let searchText: string | undefined = params['q'];
      let keysetPaginationDirection: KeysetPaginationDirection | undefined = params['paginationDirection'];
      let reference: string | undefined = params['reference'];
      let paginationToken : ParsedPaginationToken = {
        direction: keysetPaginationDirection,
        reference: reference
      };
      return this.productStoreService.search(searchText, paginationToken);
    }));
  }

  chunkRows(result: ProductMinimalDtoPagedResult): Array<Array<ProductMinimalDto>> {
    if (!result.items) {
      return [];
    }
    const chunkSize = ProductsComponent.ProductsPerRow;
    let chunkedArray = [];
    for (let i = 0; i < result.items.length; i += chunkSize) {
      const chunk = result.items.slice(i, i + chunkSize);
      chunkedArray.push(chunk);
    }
    return chunkedArray;
  }
}
