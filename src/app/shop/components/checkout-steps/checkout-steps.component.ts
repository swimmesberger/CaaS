import {Component, Input} from '@angular/core';
import {StepInfoDto} from "./step-info-dto";
import {TenantIdService} from "../../../shared/tenant-id.service";

@Component({
  selector: 'app-checkout-steps',
  templateUrl: './checkout-steps.component.html',
  styleUrls: ['./checkout-steps.component.scss']
})
export class CheckoutStepsComponent {
  @Input() steps: Array<StepInfoDto> = [];
  private activeElementIdx?: number;

  constructor(protected tenantService: TenantIdService) {
  }

  onActiveChange(elementIdx: number, active: boolean): void{
    if (active) {
      this.activeElementIdx = elementIdx;
    }
  }

  isActive(elementIdx: number): boolean {
    if (!this.activeElementIdx) return false;
    return elementIdx <= this.activeElementIdx;
  }
}
