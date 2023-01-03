import {Component, OnDestroy} from '@angular/core';
import {ProductStoreService} from "../../shared/product/product-store.service";
import {Observable, Subscription, switchMap} from "rxjs";
import {ProductMinimalDtoPagedResult} from "../../shared/product/models/productMinimalDtoPagedResult";
import {ProductMinimalDto} from "../../shared/product/models/productMinimalDto";
import {ActivatedRoute, Params, Router} from "@angular/router";
import {KeysetPaginationDirection} from "../../shared/product/models/keysetPaginationDirection";
import {ParsedPaginationToken} from "../../shared/product/models/parsedPaginationToken";
import {ProductSearchService} from "../../shared/product/product-search.service";

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnDestroy {
  private static readonly ProductsPerRow = 4;
  private _searchTextSub: Subscription;
  protected $productPage: Observable<ProductMinimalDtoPagedResult>;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private productStoreService: ProductStoreService,
    private productSearchService: ProductSearchService) {
    this._searchTextSub = this.productSearchService.searchText.subscribe(this.onSearchTextChanged.bind(this));
    this.$productPage = this.route.queryParams.pipe(switchMap(this.onQueryParamsChanged.bind(this)));
  }

  ngOnDestroy() {
    this._searchTextSub?.unsubscribe();
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

  hasRows(result: ProductMinimalDtoPagedResult): boolean {
    return (result.items?.length ?? 0) > 0;
  }

  private onSearchTextChanged(text: string | null) {
    let params: Params | null = {
      q: undefined
    };
    if (text) {
      params = {
        q: text
      }
    }
    // noinspection JSIgnoredPromiseFromCall
    this.router.navigate([],{
        relativeTo: this.route,
        queryParams: params,
        queryParamsHandling: 'merge', // remove to replace all query params by provided
      });
  }

  private onQueryParamsChanged(params: Params) {
    let searchText: string | undefined = params['q'];
    let keysetPaginationDirection: KeysetPaginationDirection | undefined = params['paginationDirection'];
    let reference: string | undefined = params['reference'];
    let paginationToken : ParsedPaginationToken = {
      direction: keysetPaginationDirection,
      reference: reference
    };
    return this.productStoreService.search(searchText, paginationToken);
  }
}
