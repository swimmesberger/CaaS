import {Component, ElementRef, QueryList, ViewChildren} from '@angular/core';
import {lastValueFrom, Observable} from "rxjs";
import {DiscountSettingDto} from "../../shared/models/discountSettingDto";
import {DiscountSettingStoreService} from "../../shared/discount-setting-store.service";
import {ActivatedRoute, Router} from "@angular/router";
import {TenantIdService} from "../../../../../shared/tenant-id.service";

@Component({
  selector: 'app-discounts',
  templateUrl: './discounts.component.html',
  styleUrls: ['./discounts.component.scss']
})
export class DiscountsComponent {
  public $discounts: Observable<Array<DiscountSettingDto>>;
  @ViewChildren("selectionCbElement") selectionCbElement!: QueryList<ElementRef>;
  isLoading: boolean = false;

  constructor(private router: Router,
              private route: ActivatedRoute,
              protected tenantIdService: TenantIdService,
              private discountSettingStore: DiscountSettingStoreService) {
    this.$discounts = discountSettingStore.getDiscountSettings();
  }

  async deleteSelected(e: Event): Promise<void> {
    e.preventDefault();
    e.stopPropagation();

    this.isLoading = true;
    try {
      for (let cbElement of this.selectionCbElement) {
        if (!(cbElement.nativeElement instanceof HTMLInputElement)) continue;
        const settingId = cbElement.nativeElement.getAttribute('data-discount-id');
        if (!settingId) continue;
        const isSelected: boolean = cbElement.nativeElement.checked;
        if (isSelected) {
          await lastValueFrom(this.discountSettingStore.deleteDiscountSettings(settingId));
        }
      }
      await this.refreshComponent();
    } finally {
      this.isLoading = false;
    }
  }

  private async refreshComponent(): Promise<void> {
    const url = this.router.url;
    // refresh
    await this.router.navigateByUrl('/', { skipLocationChange: true })
    // noinspection ES6MissingAwait
    this.router.navigateByUrl(url);
  }
}
