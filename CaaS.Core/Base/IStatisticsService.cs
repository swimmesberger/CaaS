namespace CaaS.Core.Base; 

public record MostSoldProductResult(Guid Id, long Amount);
public record CartStatisticsResult(long CountCarts, decimal SumCartsValue, decimal CountProductsInCarts, decimal AvgValueCarts, decimal AvgValuesInCart);

public interface IStatisticsService {
    
    /// <summary>
    /// Gets the most sold product (by sold pieces) within a certain period of time.
    /// </summary>
    /// <param name="from">start of period</param>
    /// <param name="until">end of period</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A MostSoldProductResult object which contains the productID and the number of sold pieces</returns>
    Task<MostSoldProductResult> GetMostSoldProduct(DateTimeOffset from, DateTimeOffset until, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns the average value of orders within a certain period of time considering item discounts, coupons and order discounts
    /// </summary>
    /// <param name="from">start of period considered for analysis</param>
    /// <param name="until">end of period considered for analysis</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The average value of orders</returns>
    Task<decimal> AverageDiscountedValueOfOrdersInTimePeriod(DateTimeOffset from, DateTimeOffset until, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns the average value of orders within a certain period of time without considering dicounts or coupons
    /// Does not considering any item discounts, coupons and order discounts
    /// </summary>
    /// <param name="from">start of period considered for analysis</param>
    /// <param name="until">end of period considered for analysis</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The average value of orders</returns>
    Task<decimal> AverageValueOfOrdersInTimePeriod(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns statistics about carts within a certain period of time. Discounts and coupons are NOT considered!
    /// </summary>
    /// <param name="from">start of period</param>
    /// <param name="until">end of period</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A CartStatisticsResult object</returns>
    Task<CartStatisticsResult> GetCartStatistics(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default);
}