export interface OrderStatisticsResultDateAggregateDto {
  date?: string,
  countOrders?: number,
  sumOfOrders?: number,
  discountedSumOfOrders?: number,
  avgValueOfOrders?: number,
  discountedAvgValueOfOrders?: number,
  sumProducts?: number,
  avgNumberOfProductsInOrders?: number
}
