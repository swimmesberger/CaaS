import {AfterViewInit, Component, ElementRef, Input, NgZone, QueryList, ViewChild, ViewChildren} from '@angular/core';
import {AbstractControl, FormControl, FormGroup, Validators} from "@angular/forms";
import {CreditCardDataDto} from "../../shared/order/models/customerWithAddressDto";

// @ts-ignore
declare var Card;

@Component({
  selector: 'app-creditcard',
  templateUrl: './creditcard.component.html',
  styleUrls: ['./creditcard.component.scss']
})
export class CreditcardComponent implements AfterViewInit {
  @ViewChild('formElement') formElement!: ElementRef;
  @ViewChildren('inputElement') inputElements!: QueryList<ElementRef>;

  protected creditCardFormGroup: FormGroup<CreditCardForm>;

  constructor(private _ngZone: NgZone) {
    this.creditCardFormGroup = new FormGroup<CreditCardForm>({
      creditCardNumber: new FormControl<string | null>(null, Validators.required),
      cvc: new FormControl<string | null>(null, Validators.required),
      fullName: new FormControl<string | null>(null, Validators.required),
      validDate: new FormControl<string | null>(null, Validators.required)
    });
  }

  public get valid(): boolean {
    return this.creditCardFormGroup.valid;
  }

  @Input()
  public set creditCardData(data: CreditCardDataDto | null) {
    // @ts-ignore
    if (data) {
      this.creditCardFormGroup.setValue(data);
    } else {
      this.creditCardFormGroup.reset();
    }
  }

  public get creditCardData(): CreditCardDataDto | null {
    if(this.creditCardFormGroup.invalid) return null;
    // @ts-ignore
    return this.creditCardFormGroup.value;
  }

  ngAfterViewInit() {
    this._ngZone.runOutsideAngular(() => {
      new Card({
        form: this.formElement.nativeElement,
        container: '.credit-card-wrapper'
      });
    });
  }

  isInvalid(control: AbstractControl) {
    return control.invalid && control.touched;
  }

  get creditCardNumberControl() { return this.creditCardFormGroup.get('creditCardNumber')!; }
  get cvcControl() { return this.creditCardFormGroup.get('cvc')!; }
  get fullNameControl() { return this.creditCardFormGroup.get('fullName')!; }
  get validDateControl() { return this.creditCardFormGroup.get('validDate')!; }
}

interface CreditCardForm {
  creditCardNumber: FormControl<string|null>,
  fullName: FormControl<string|null>,
  validDate: FormControl<string|null>,
  cvc: FormControl<string|null>
}

