using System.Data;
using CaaS.Core.Base;
using CaaS.Core.Base.Tenant;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;

namespace CaaS.Infrastructure.ProductData;

public class StatisticsImpl : IStatisticsService {
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly IStatementExecutor _statementExecutor;

    public StatisticsImpl(ITenantIdAccessor tenantIdAccessor, IStatementExecutor statementExecutor) {
        _tenantIdAccessor = tenantIdAccessor;
        _statementExecutor = statementExecutor;
    }

    public async Task<MostSoldProductResult> GetMostSoldProduct(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default) {

        string sql = "select " +
                     "po.product_id as product_id, " +
                     "sum(po.amount) as sum " +
                     "from " +
                     "\"product_order\" po join \"order\" o ON (po.order_id = o.id) " +
                     "where " +
                     "o.shop_id = @shop_id AND order_date >= @from and order_date <= @until group by po.product_id" +
                     " order by " +
                     "sum(po.amount) DESC " +
                     "limit 1";

        MaterializedStatements statements = new MaterializedStatements(new MaterializedStatement(sql) {
            Parameters = new[] {
                QueryParameter.From("shop_id", _tenantIdAccessor.GetTenantGuid()),
                QueryParameter.From("from", from),
                QueryParameter.From("until", until),
            }
        });

        return await _statementExecutor.StreamAsync(statements,
            async (record, token) => new MostSoldProductResult(
                await record.GetFieldValueAsync<Guid>("product_id", token),
                await record.GetFieldValueAsync<long>("sum", token)
            ), cancellationToken).FirstAsync(cancellationToken);
    }

    public async Task<decimal> AverageDiscountedValueOfOrdersInTimePeriod(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default) {

        string sql =
            "SELECT AVG(order_value) " +
            "FROM( " +
            "SELECT " +
            "order_discounted_sum, " +
            "c.coupons_sum, " +
            "order_discounts.order_discount_sum, " +
            "order_discounted_sum - COALESCE(c.coupons_sum,0) - COALESCE(order_discounts.order_discount_sum,0) as order_value " +
            "FROM( " +
            "SELECT po.order_id," +
            "SUM(amount*price_per_piece) as order_sum," +
            "SUM(amount*(price_per_piece-COALESCE(discount,0))) as order_discounted_sum " +
            "FROM (" +
            "SELECT " +
            "product_order_id," +
            "SUM(discount) as discount " +
            "FROM " +
            "product_order_discount " +
            "GROUP BY " +
            "product_order_id " +
            "ORDER BY " +
            "product_order_id) pod RIGHT JOIN product_order po ON (pod.product_order_id = po.id)" +
            "WHERE " +
            "shop_id = @shop_id " +
            "GROUP BY po.order_id " +
            "ORDER BY order_id) order_statistic " +
            "JOIN \"order\" o ON (order_statistic.order_id = o.id) " +
            "LEFT JOIN (SELECT order_id, SUM(value) as coupons_sum " +
            "FROM coupon " +
            "GROUP BY order_id) c ON (order_statistic.order_id = c.order_id)" +
            "LEFT JOIN (SELECT order_id, SUM(discount) as order_discount_sum " +
            "FROM order_discount " +
            "GROUP BY order_id) order_discounts ON (order_statistic.order_id = order_discounts.order_id)" +
            "WHERE " +
            "order_date >= @from " +
            "AND order_date <= @until " +
            "ORDER BY order_statistic.order_id) statistics";

        MaterializedStatements statements = new MaterializedStatements(new MaterializedStatement(sql) {
            Parameters = new[] {
                QueryParameter.From("shop_id", _tenantIdAccessor.GetTenantGuid()),
                QueryParameter.From("from", from),
                QueryParameter.From("until", until),
            }
        });

        var result = (decimal)(await _statementExecutor.QueryScalarAsync(statements, cancellationToken) ?? 0);
        return result;
    }
    
    public async Task<decimal> AverageValueOfOrdersInTimePeriod(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default) {

        string sql = "SELECT AVG(i.sum)" +
                     "FROM( " +
                     "SELECT " +
                     "SUM(amount*price_per_piece) as sum " +
                     "FROM " +
                     "product_order po " +
                     "JOIN \"order\" o ON (po.order_id = o.id) " +
                     "WHERE " +
                     "po.shop_id = @shop_id " +
                     "AND order_date >= @from  " +
                     "AND order_date <= @until " +
                     "GROUP BY order_id " +
                     "ORDER BY order_id) as i ";


        MaterializedStatements statements = new MaterializedStatements(new MaterializedStatement(sql) {
            Parameters = new[] {
                QueryParameter.From("shop_id", _tenantIdAccessor.GetTenantGuid()),
                QueryParameter.From("from", from),
                QueryParameter.From("until", until),
            }
        });

        var result = (decimal)(await _statementExecutor.QueryScalarAsync(statements, cancellationToken) ?? 0);
        return result;
    }

    public async Task<CartStatisticsResult> GetCartStatistics(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default) {

        string sql = "SELECT " +
                         "COUNT(base.cart_id) as count_carts, " +
                         "SUM(base.sum_cart) as sum_value_carts, " +
                         "SUM(base.count_products_in_cart) as count_products_in_carts, " +
                         "AVG(base.sum_cart) as avg_value_carts, " +
                         "AVG(base.count_products_in_cart) as avg_products_in_carts	" +
                     "FROM " +
                         "(SELECT " +
                             "cart_id, " +
                             "SUM(amount*price) as sum_cart, " +
                             "SUM(amount) as count_products_in_cart " +
                         "FROM " +
                            "product_cart pc " +
                         "JOIN product p ON (pc.product_id = p.id) " +
                         "JOIN cart c ON (pc.cart_id = c.id) " +
                         "WHERE " +
                             "pc.shop_id = @shop_id " +
                             "AND pc.creation_time >= @from " +
                             "AND pc.creation_time <= @until " +
                         "GROUP BY " +
                            "pc.cart_id) as base";
        
        MaterializedStatements statements = new MaterializedStatements(new MaterializedStatement(sql) {
            Parameters = new[] {
                QueryParameter.From("shop_id", _tenantIdAccessor.GetTenantGuid()),
                QueryParameter.From("from", from),
                QueryParameter.From("until", until),
            }
        });

        return await _statementExecutor.StreamAsync(statements,
            async (record, token) => new CartStatisticsResult(
                await record.GetFieldValueAsync<long>("count_carts", token),
                await record.GetFieldValueAsync<decimal>("sum_value_carts", token),
                await record.GetFieldValueAsync<decimal>("count_products_in_carts", token),
                await record.GetFieldValueAsync<decimal>("avg_value_carts", token),
                await record.GetFieldValueAsync<decimal>("avg_products_in_carts", token)
            ), cancellationToken).FirstAsync(cancellationToken);
    }

}
