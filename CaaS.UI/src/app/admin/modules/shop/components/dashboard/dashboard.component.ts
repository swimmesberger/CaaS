import {AfterViewInit, Component} from '@angular/core';
import {FormControl, FormGroup} from '@angular/forms';

import {IChartProps} from "./chart-props";
import {DashboardChartsData} from "../../shared/dashboard-charts-data.service";
import {from, Observable, startWith, switchMap} from "rxjs";
import {OverallStatisticsResultDto} from "../../shared/models/overallStatisticsResultDto";

@Component({
  templateUrl: 'dashboard.component.html',
  styleUrls: ['dashboard.component.scss']
})
export class DashboardComponent implements AfterViewInit {
  public topProductsChart: IChartProps = {
    type: 'bar',
    options: {}
  };
  public revenueChart: IChartProps = {
    type: 'line',
    options: {}
  };
  public soldProductsChart: IChartProps = {
    type: 'line',
    options: {}
  };
  public numOfCartChart: IChartProps = {
    type: 'line',
    options: {}
  };
  public valueOfCartChart: IChartProps = {
    type: 'line',
    options: {}
  };
  public topProductsForm: FormGroup;
  public revenueForm: FormGroup;
  public soldProductsForm: FormGroup;
  public numberOfCartForm: FormGroup;
  public valueOfCartForm: FormGroup;
  public overallForm: FormGroup;
  public $overallStats: Observable<OverallStatisticsResultDto>;

  constructor(private chartProvider: DashboardChartsData) {
    const year = new Date().getFullYear()-1;
    const startOfYear = new Date(year, 0, 1, 23, 59, 59).toISOString().substring(0, 10);
    const endOfYear = new Date(year, 11, 31, 23, 59, 59).toISOString().substring(0, 10);
    this.topProductsForm = new FormGroup({
      from: new FormControl<string>(startOfYear),
      to: new FormControl<string>(endOfYear)
    });
    this.topProductsForm.valueChanges.pipe(switchMap(() =>
      from(this.initTopProductsChart()))).subscribe();
    this.revenueForm = new FormGroup({
      from: new FormControl<string>(startOfYear),
      to: new FormControl<string>(endOfYear),
      aggregateType: new FormControl<string>('Month')
    });
    this.revenueForm.valueChanges.pipe(switchMap(() =>
      from(this.initRevenueChart()))).subscribe();
    this.soldProductsForm = new FormGroup({
      from: new FormControl<string>(startOfYear),
      to: new FormControl<string>(endOfYear),
      aggregateType: new FormControl<string>('Month')
    });
    this.soldProductsForm.valueChanges.pipe(switchMap(() =>
      from(this.initSoldProductsChart()))).subscribe();
    this.numberOfCartForm = new FormGroup({
      from: new FormControl<string>(startOfYear),
      to: new FormControl<string | null>(null),
      aggregateType: new FormControl<string>('Month')
    });
    this.numberOfCartForm.valueChanges.pipe(switchMap(() =>
      from(this.initNumberOfCartChart()))).subscribe();
    this.valueOfCartForm = new FormGroup({
      from: new FormControl<string>(startOfYear),
      to: new FormControl<string | null>(null),
      aggregateType: new FormControl<string>('Month')
    });
    this.valueOfCartForm.valueChanges.pipe(switchMap(() =>
      from(this.initValueOfCartChart()))).subscribe();
    this.overallForm = new FormGroup({
      from: new FormControl<string>(startOfYear),
      to: new FormControl<string | null>(null)
    });
    this.$overallStats = this.overallForm.valueChanges.pipe(startWith(undefined), switchMap(() =>
      this.chartProvider.getOverall(
        new Date(this.overallForm.get('from')?.value),
        this.overallForm.get('to')?.value ? new Date(this.overallForm.get('to')?.value) : null
      )));
  }

  ngAfterViewInit() {
    (async () => {
      await this.initTopProductsChart();
      await this.initRevenueChart();
      await this.initSoldProductsChart();
      await this.initNumberOfCartChart();
      await this.initValueOfCartChart();
    })();
  }

  private async initTopProductsChart(): Promise<void> {
    this.topProductsChart = await this.chartProvider.getTopProducts(
      new Date(this.topProductsForm.get('from')?.value),
      new Date(this.topProductsForm.get('to')?.value)
    );
  }

  private async initRevenueChart(): Promise<void> {
    try {
      this.revenueChart = await this.chartProvider.getRevenue(
        new Date(this.revenueForm.get('from')?.value),
        new Date(this.revenueForm.get('to')?.value),
        this.revenueForm.get('aggregateType')?.value
      );
    } catch(e) {
      console.error(e);
      this.revenueChart = {
        type: 'line',
        options: {}
      };
    }
  }

  private async initSoldProductsChart(): Promise<void> {
    try {
      this.soldProductsChart = await this.chartProvider.getNumberOfSales(
        new Date(this.soldProductsForm.get('from')?.value),
        new Date(this.soldProductsForm.get('to')?.value),
        this.soldProductsForm.get('aggregateType')?.value
      );
    } catch(e) {
      console.error(e);
      this.soldProductsChart = {
        type: 'line',
        options: {}
      };
    }
  }

  private async initNumberOfCartChart(): Promise<void> {
    try {
      this.numOfCartChart = await this.chartProvider.getNumberOfCart(
        new Date(this.numberOfCartForm.get('from')?.value),
        this.numberOfCartForm.get('to')?.value ? new Date(this.numberOfCartForm.get('to')?.value) : null,
        this.numberOfCartForm.get('aggregateType')?.value
      );
    } catch(e) {
      console.error(e);
      this.numOfCartChart = {
        type: 'line',
        options: {}
      };
    }
  }

  private async initValueOfCartChart(): Promise<void> {
    try {
      this.valueOfCartChart = await this.chartProvider.getValueOfCart(
        new Date(this.valueOfCartForm.get('from')?.value),
        this.valueOfCartForm.get('to')?.value ? new Date(this.valueOfCartForm.get('to')?.value) : null,
        this.valueOfCartForm.get('aggregateType')?.value
      );
    } catch(e) {
      console.error(e);
      this.valueOfCartChart = {
        type: 'line',
        options: {}
      };
    }
  }
}
