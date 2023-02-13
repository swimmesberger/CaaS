// noinspection AllyPlainJsInspection

import {Injectable} from "@angular/core";
import {IChartProps} from "../components/dashboard/chart-props";
import {StatisticsService} from "./statistics.service";
import {combineLatest, lastValueFrom, map, Observable} from "rxjs";
import {OverallStatisticsResultDto} from "./models/overallStatisticsResultDto";

@Injectable({
  providedIn: 'any'
})
export class DashboardChartsData {
  private readonly months = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
  ];

  constructor(private statisticsService: StatisticsService) {}

  async getTopProducts(from: Date, to: Date): Promise<IChartProps> {
    const mostSoldProductsData = await lastValueFrom(this.statisticsService.getMostSoldProducts(from, to))
    const labels = mostSoldProductsData.map(d => d.productName);
    const data = mostSoldProductsData.map(d => d.amount);
    return {
      type: 'bar',
      data: {
        labels: labels,
        datasets: [{
          label: 'Products',
          data: data,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgb(54, 162, 235)',
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          y: {
            beginAtZero: true
          }
        }
      },
    };
  }

  async getRevenue(from: Date, to: Date, aggregate: string): Promise<IChartProps> {
    const orderData = await lastValueFrom(this.statisticsService.getOrderStatistics(from, to, aggregate))
    const labels = aggregate == 'Month' ?
      orderData.map(o => this.months[new Date(o.date!).getMonth()]) :
      orderData.map(o => new Date(o.date!).getFullYear());
    const data = orderData.map(d => d.discountedSumOfOrders);
    return {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: 'Revenue',
          data: data,
          fill: false,
          borderColor: 'rgb(54, 162, 235)',
          tension: 0.1
        }]
      }
    };
  }

  async getNumberOfSales(from: Date, to: Date, aggregate: string): Promise<IChartProps> {
    const orderData = await lastValueFrom(this.statisticsService.getOrderStatistics(from, to, aggregate))
    const labels = aggregate == 'Month' ?
      orderData.map(o => this.months[new Date(o.date!).getMonth()]) :
      orderData.map(o => new Date(o.date!).getFullYear());
    const data = orderData.map(d => d.sumProducts);
    return {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: 'Number of products',
          data: data,
          fill: false,
          borderColor: 'rgb(54, 162, 235)',
          tension: 0.1
        }]
      }
    };
  }

  async getNumberOfCart(from: Date, to: Date | null, aggregate: string): Promise<IChartProps> {
    const cartData = await lastValueFrom(this.statisticsService.getCartStatistics(from, to, aggregate))
    const labels = aggregate == 'Month' ?
      cartData.map(o => this.months[new Date(o.date!).getMonth()]) :
      cartData.map(o => new Date(o.date!).getFullYear());
    const numCartsData = cartData.map(d => d.countCarts);
    return {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: 'Number of carts',
          data: numCartsData,
          fill: false,
          borderColor: 'rgb(54, 162, 235)',
          tension: 0.1
        }]
      }
    };
  }

  async getValueOfCart(from: Date, to: Date | null, aggregate: string): Promise<IChartProps> {
    const cartData = await lastValueFrom(this.statisticsService.getCartStatistics(from, to, aggregate))
    const labels = aggregate == 'Month' ?
      cartData.map(o => this.months[new Date(o.date!).getMonth()]) :
      cartData.map(o => new Date(o.date!).getFullYear());
    const valueCartsData = cartData.map(d => d.sumCartsValue);
    return {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: 'Value of carts',
          data: valueCartsData,
          fill: false,
          borderColor: 'rgb(54, 162, 235)',
          tension: 0.1
        }]
      }
    };
  }

  getOverall(from: Date, to: Date | null = null): Observable<OverallStatisticsResultDto> {
    return combineLatest([
      this.statisticsService.getOverallCartStatistics(from, to),
      this.statisticsService.getOverallCouponStatistics(from, to)
    ]).pipe(map(([cartStat, couponStat]) => {
      return <OverallStatisticsResultDto>{
        ...cartStat,
        ...couponStat
      };
    }));
  }
}
