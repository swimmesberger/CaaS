namespace CaaS.Core.Base; 

public record MostSoldProductResult(Guid ProductId, long Amount);
public record CartStatisticsResult(long CountCarts, decimal SumCartsValue,  decimal AvgValueOfCarts, decimal CountProductsInCarts, decimal AvgProductsInCart);
public record CartStatisticsResultDateAggregate(DateTimeOffset Date, long CountCarts, decimal SumCartsValue,  decimal AvgValueOfCarts, decimal CountProductsInCarts, decimal AvgProductsInCart);
public record OrderStatisticsResultDateAggregate(DateTimeOffset Date, int CountOrders, decimal SumOfOrders, decimal DiscountedSumOfOrders, decimal AvgValueOfOrders,
                                    decimal DiscountedAvgValueOfOrders, int SumProducts, decimal AvgNumberOfProductsInOrders);

public enum AggregateByDatePart {
    Day,
    Month,
    Year
}

public interface IStatisticsService {
    
    /// <summary>
    /// Gets the most sold product (by sold pieces) within a certain period of time.
    /// </summary>
    /// <param name="from">start of period</param>
    /// <param name="until">end of period</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A MostSoldProductResult object which contains the productID and the number of sold pieces</returns>
    Task<MostSoldProductResult> MostSoldProductOverall(DateTimeOffset from, DateTimeOffset until, CancellationToken cancellationToken = default);
    
   /// <summary>
    /// Returns the average value of orders within a certain period of time considering item discounts, coupons and order discounts AGGREGATED by a certain value
    /// </summary>
    /// <param name="from">start of period considered for analysis</param>
    /// <param name="until">end of period considered for analysis</param>
    /// <param name="aggregate">day, month, or year</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of orderStatisticsResults for date-aggregated data</returns>
    Task<IReadOnlyCollection<OrderStatisticsResultDateAggregate>> OrderStatisticsAggregatedByDate(DateTimeOffset from, DateTimeOffset until,
        AggregateByDatePart aggregate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns overall statistics about carts within a certain period of time. Discounts and coupons are NOT considered!
    /// </summary>
    /// <param name="from">start of period</param>
    /// <param name="until">end of period</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A CartStatisticsResult object</returns>
    Task<CartStatisticsResult> CartStatisticsOverall(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns cart statistics grouped by creation_time of cart. Discounts and coupons are NOT considered!
    /// </summary>
    /// <param name="from">start of period</param>
    /// <param name="until">end of period</param>
    /// <param name="aggregate">day, month, or year</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A List of CartStatisticsResult object</returns>
    Task<IReadOnlyCollection<CartStatisticsResultDateAggregate>> CartStatisticsAggregatedByDate(DateTimeOffset from, DateTimeOffset until,
        AggregateByDatePart aggregate, CancellationToken cancellationToken = default);
}