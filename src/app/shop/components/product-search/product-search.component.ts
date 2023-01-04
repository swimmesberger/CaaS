import {AfterViewInit, Component, ElementRef, HostBinding, Input, OnDestroy, ViewChild,} from '@angular/core';
import {ProductSearchService} from "../../shared/product/product-search.service";
import {SearchBarStyle} from "./search-bar-style";
import {ActivatedRoute, Params, Router} from "@angular/router";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-product-search',
  templateUrl: './product-search.component.html',
  styleUrls: ['./product-search.component.scss'],
  host: {
    class: 'input-group'
  }
})
export class ProductSearchComponent implements OnDestroy, AfterViewInit {
  @Input() searchBarStyle: string = SearchBarStyle.Large;
  @ViewChild('searchInputElement') searchInputElement?: ElementRef;
  private _queryParamsSub: Subscription;
  constructor(private router: Router,
              private route: ActivatedRoute,
              private productSearchService: ProductSearchService) {
    this._queryParamsSub = this.route.queryParams.subscribe(this.onQueryParamsChanged.bind(this));
  }

  ngAfterViewInit() {
    this.onQueryParamsChanged(this.route.snapshot.queryParams);
  }

  ngOnDestroy() {
    this._queryParamsSub?.unsubscribe();
  }

  onSearchTextChanged(e: Event): void {
    if (e.target instanceof HTMLInputElement) {
      e.target.value;
      this.productSearchService.setSearchText(e.target.value);
    }
  }

  onSubmitSearch(e: Event): void {
    if (!(e.target instanceof HTMLInputElement) || !e.target.value) {
      return;
    }
    e.preventDefault();
    e.stopPropagation();
    // noinspection JSIgnoredPromiseFromCall
    this.router.navigate(['/products'],{
      queryParams: {q: e.target.value},
      queryParamsHandling: 'merge', // remove to replace all query params by provided
    });
  }

  private onQueryParamsChanged(params: Params) {
    if (!this.searchInputElement) return;
    this.searchInputElement.nativeElement.value = params['q'] ?? '';
  }

  @HostBinding("class.d-lg-none")
  @HostBinding("class.my-3")
  get isSmall() {
    return this.searchBarStyle === 'small';
  }

  @HostBinding("class.d-none")
  @HostBinding("class.d-lg-flex")
  @HostBinding("class.mx-4")
  get isLarge() {
    return this.searchBarStyle === 'large';
  }
}
