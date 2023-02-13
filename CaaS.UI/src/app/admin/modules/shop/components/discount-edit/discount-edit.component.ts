import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {lastValueFrom, Observable, of, Subscription, switchMap} from "rxjs";
import {DiscountSettingDto} from "../../shared/models/discountSettingDto";
import {ActivatedRoute, Router} from "@angular/router";
import {DiscountSettingStoreService} from "../../shared/discount-setting-store.service";
import {TenantIdService} from "../../../../../shared/tenant-id.service";

@Component({
  selector: 'app-discount-edit',
  templateUrl: './discount-edit.component.html',
  styleUrls: ['./discount-edit.component.scss']
})
export class DiscountEditComponent implements OnInit, OnDestroy {
  @Input() discountId: string | null;
  discountForm: FormGroup<DiscountSettingForm|undefined>;

  private _settingSub: Subscription | undefined;
  constructor(private router: Router,
              private route: ActivatedRoute,
              protected tenantIdService: TenantIdService,
              private discountSettingStore: DiscountSettingStoreService) {
    this.discountId = null;
    this.discountForm = new FormGroup<DiscountSettingForm|undefined>(<DiscountSettingForm>{
      id: new FormControl<string | undefined>(undefined),
      name: new FormControl<string | undefined>(undefined, Validators.required),
      action: new FormGroup<DiscountMetadataSettingForm>(<DiscountMetadataSettingForm>{
        id: new FormControl<string | undefined>(undefined),
        name: new FormControl<string | undefined>(undefined),
        version: new FormControl<number | undefined>(undefined),
        parameters: new FormControl<string | undefined>(undefined)
      }),
      rule: new FormGroup<DiscountMetadataSettingForm>(<DiscountMetadataSettingForm>{
        id: new FormControl<string | undefined>(undefined),
        name: new FormControl<string | undefined>(undefined),
        version: new FormControl<number | undefined>(undefined),
        parameters: new FormControl<string | undefined>(undefined)
      }),
      shopId: new FormControl<string | undefined>(undefined),
      concurrencyToken: new FormControl<string | undefined>(undefined),
    });
  }

  ngOnInit(): void {
    let $setting: Observable<DiscountSettingDto | null>;
    if (this.discountId == null) {
      $setting = this.route.params.pipe(switchMap(params => {
        const discountId = params['discountId'];
        if (!discountId) return of(null);
        return this.discountSettingStore.getDiscountSettingById(discountId);
      }));
    } else {
      $setting = this.discountSettingStore.getDiscountSettingById(this.discountId);
    }
    this._settingSub = $setting.subscribe(setting => {
      if (setting === null) {
        this.discountId = null;
        this.discountForm.reset();
        return;
      }
      const actionParams = {
        ...setting.action?.parameters
      }
      const ruleParams = {
        ...setting.rule?.parameters
      }
      const actionStringParams = JSON.stringify({
        ...actionParams,
        name: undefined,
        version: undefined
      });
      const ruleStringParams = JSON.stringify({
        ...ruleParams,
        name: undefined,
        version: undefined
      });

      this.discountId = setting.id ?? null;
      // @ts-ignore
      this.discountForm.setValue({
        ...setting,
        action: {
          id: setting.action?.id,
          name: actionParams.name,
          version: actionParams.version,
          parameters: actionStringParams
        },
        rule: {
          id: setting.rule?.id,
          name: ruleParams.name,
          version: ruleParams.version,
          parameters: ruleStringParams
        }
      });
    });
  }

  ngOnDestroy() {
    this._settingSub?.unsubscribe();
  }

  async save(e: Event): Promise<void> {
    e.preventDefault();
    e.stopPropagation();

    const formValue = this.discountForm.value;
    // @ts-ignore
    const setting: DiscountSettingDto = {
      id: formValue?.id ?? undefined,
      name: formValue?.name ?? undefined,
      concurrencyToken: formValue?.concurrencyToken ?? undefined,
      shopId: undefined,
      action: {
        id: formValue?.action?.id ?? undefined,
        parameters: {
          name: formValue?.action?.name ?? undefined,
          version: formValue?.action?.version ?? undefined,
          ...JSON.parse(formValue?.action?.parameters),
        }
      },
      rule: {
        id: formValue?.rule?.id ?? undefined,
        parameters: {
          name: formValue?.rule?.name ?? undefined,
          version: formValue?.rule?.version ?? undefined,
          ...JSON.parse(formValue?.rule?.parameters),
        }
      }
    }
    if(this.discountId == null) {
      await lastValueFrom(this.discountSettingStore.createDiscountSetting(setting));
    } else {
      await lastValueFrom(this.discountSettingStore.updateDiscountSetting(setting));
    }
    // noinspection ES6MissingAwait
    this.router.navigate([this.tenantIdService.tenantUrl + '/admin/discounts']);
  }
}

interface DiscountSettingForm {
  id: FormControl<string|undefined>;
  name: FormControl<string|undefined>;
  rule: FormGroup<DiscountMetadataSettingForm|undefined>;
  action: FormGroup<DiscountMetadataSettingForm|undefined>;
  concurrencyToken: FormControl<string|undefined>
}

interface DiscountMetadataSettingForm {
  id: FormControl<string|undefined>,
  name: FormControl<string|undefined>,
  version: FormControl<number|undefined>,
  parameters: FormControl<any|undefined>
}
