using System.Data;
using CaaS.Core.Base;
using CaaS.Core.Base.Tenant;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;

namespace CaaS.Infrastructure.Base;

public class StatisticsImpl : IStatisticsService {
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly IStatementExecutor _statementExecutor;

    public StatisticsImpl(ITenantIdAccessor tenantIdAccessor, IStatementExecutor statementExecutor) {
        _tenantIdAccessor = tenantIdAccessor;
        _statementExecutor = statementExecutor;
    }

    public async Task<MostSoldProductResult> MostSoldProductOverall(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default) {

        string sql = "select " +
                     "po.product_id as product_id, " +
                     "COALESCE(sum(po.amount),0) as sum " +
                     "from " +
                     "\"product_order\" po join \"order\" o ON (po.order_id = o.id) " +
                     "where " +
                     "o.shop_id = @shop_id AND order_date >= @from and order_date <= @until group by po.product_id" +
                     " order by " +
                     "sum(po.amount) DESC " +
                     "limit 1";

        var statement = new MaterializedStatement(sql) {
            Parameters = new QueryParameter[] {
                new("shop_id", _tenantIdAccessor.GetTenantGuid()),
                new("from", from),
                new("until", until),
            }
        };

        var result = await _statementExecutor.StreamAsync(statement,
            async (record, token) => new MostSoldProductResult(
                await record.GetFieldValueAsync<Guid>("product_id", token),
                await record.GetFieldValueAsync<long>("sum", token)
            ), cancellationToken).FirstOrDefaultAsync(cancellationToken);
        result ??= new MostSoldProductResult(Guid.Empty, 0);
        return result;
    }

    public async Task<IReadOnlyCollection<OrderStatisticsResultDateAggregate>> OrderStatisticsAggregatedByDate(DateTimeOffset from, DateTimeOffset until, 
         AggregateByDatePart aggregate, CancellationToken cancellationToken = default) {
        
        string sql =
            "SELECT " +
            $"date_trunc('{aggregate.ToString()}', order_date) as date, "  +
            "COUNT(order_statistic.order_idd) as count_orders, " +
            "SUM(order_sum) as sum_order_value, " +
            "SUM(order_discounted_sum - COALESCE(c.coupons_sum,0) - COALESCE(order_discounts.order_discount_sum,0)) as discounted_sum_order_value, " +
            "ROUND(AVG(order_sum),2) as avg_order_value, " +
            "ROUND(AVG(order_discounted_sum - COALESCE(c.coupons_sum,0) - COALESCE(order_discounts.order_discount_sum,0)),2) as avg_order_value_discounted, " +
            "SUM(sum_products) as sum_products, " +
            "ROUND(AVG(sum_products),2) as avg_products_per_order " +
            "FROM( " +
            "SELECT po.order_id," +
            "SUM(amount*price_per_piece) as order_sum," +
            "SUM(amount*(price_per_piece-COALESCE(discount,0))) as order_discounted_sum, " +
            "SUM(amount) as sum_products, " +
            "order_id as order_idd " +
            "FROM (" +
            "SELECT " +
            "product_order_id," +
            "SUM(discount) as discount " +
            "FROM " +
            "product_order_discount " +
            "GROUP BY " +
            "product_order_id " +
            "ORDER BY " +
            "product_order_id) pod RIGHT JOIN product_order po ON (pod.product_order_id = po.id) " +
            "WHERE " +
            "shop_id = @shop_id " +
            "GROUP BY po.order_id " +
            "ORDER BY order_id) order_statistic " +
            "JOIN \"order\" o ON (order_statistic.order_idd = o.id) " +
            "LEFT JOIN (SELECT order_id, SUM(value) as coupons_sum " +
            "FROM coupon " +
            "GROUP BY order_id) c ON (order_statistic.order_id = c.order_id) " +
            "LEFT JOIN (SELECT order_id, SUM(discount) as order_discount_sum " +
            "FROM order_discount " +
            "GROUP BY order_id) order_discounts ON (order_statistic.order_id = order_discounts.order_id) " +
            "WHERE " +
            "order_date >= @from " +
            "AND order_date <= @until " +
            $"GROUP BY date_trunc('{aggregate.ToString()}', order_date) ORDER BY date_trunc('{aggregate.ToString()}', order_date) " ;

        var statement = new MaterializedStatement(sql) {
            Parameters = new QueryParameter[] {
                new("shop_id", _tenantIdAccessor.GetTenantGuid()),
                new("from", from),
                new("until", until),
            }
        };

        return await _statementExecutor.StreamAsync(statement,
            async (record, token) => new OrderStatisticsResultDateAggregate(
                await record.GetFieldValueAsync<DateTimeOffset>("date", token),
                await record.GetFieldValueAsync<int>("count_orders", token),
                await record.GetFieldValueAsync<decimal>("sum_order_value", token),
                await record.GetFieldValueAsync<decimal>("discounted_sum_order_value", token),
                await record.GetFieldValueAsync<decimal>("avg_order_value", token),
                await record.GetFieldValueAsync<decimal>("avg_order_value_discounted", token),
                await record.GetFieldValueAsync<int>("sum_products", token),
                await record.GetFieldValueAsync<decimal>("avg_products_per_order", token)
            ), cancellationToken).ToListAsync(cancellationToken);
     }
    
    public async Task<CartStatisticsResult> CartStatisticsOverall(DateTimeOffset from, DateTimeOffset until,
        CancellationToken cancellationToken = default) {

        string sql = "SELECT " +
                         "COALESCE(COUNT(base.cart_id),0) as count_carts, " +
                         "COALESCE(SUM(base.sum_cart),0) as sum_value_carts, " +
                         "COALESCE(SUM(base.count_products_in_cart),0) as count_products_in_carts, " +
                         "COALESCE(AVG(base.sum_cart),0) as avg_value_carts, " +
                         "COALESCE(AVG(base.count_products_in_cart),0) as avg_products_in_carts	" +
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
            Parameters = new QueryParameter[] {
                new("shop_id", _tenantIdAccessor.GetTenantGuid()),
                new("from", from),
                new("until", until),
            }
        });

        return await _statementExecutor.StreamAsync(statements,
            async (record, token) => new CartStatisticsResult(
                await record.GetFieldValueAsync<long>("count_carts", token),
                await record.GetFieldValueAsync<decimal>("sum_value_carts", token),
                await record.GetFieldValueAsync<decimal>("avg_value_carts", token),
                await record.GetFieldValueAsync<decimal>("count_products_in_carts", token),
                await record.GetFieldValueAsync<decimal>("avg_products_in_carts", token)
            ), cancellationToken).FirstAsync(cancellationToken);
    }
    
    
    public async Task<IReadOnlyCollection<CartStatisticsResultDateAggregate>> CartStatisticsAggregatedByDate(DateTimeOffset from, DateTimeOffset until, 
                    AggregateByDatePart aggregate, CancellationToken cancellationToken = default) {
        
        string sql = "SELECT  " +
                     $"date_trunc('{aggregate.ToString()}', base.time) as cart_creation_date, " +
                     "COALESCE(COUNT(base.cart_id),0) as count_carts, " +
                     "COALESCE(SUM(base.sum_cart),0) as sum_value_carts, " +
                     "ROUND(COALESCE(AVG(base.sum_cart),0),2) as avg_value_carts, " +
                     "COALESCE(SUM(base.count_products_in_cart),0) as count_products_in_carts, " +
                     "ROUND(COALESCE(AVG(base.count_products_in_cart),0),2) as avg_products_in_carts " +
                     "FROM " +
                     "(SELECT " +
                     "cart_id, " +
                     "SUM(amount*price) as sum_cart, " +
                     "SUM(amount) as count_products_in_cart , " +
                     "c.creation_time as time " +
                     "FROM  " +
                     "product_cart pc  " +
                     "JOIN product p ON (pc.product_id = p.id) " +
                     "JOIN cart c ON (pc.cart_id = c.id) " +
                     "WHERE " +
                     "pc.shop_id = @shop_id " +
                     "AND pc.creation_time >= @from " +
                     "AND pc.creation_time <= @until  " +
                     "GROUP BY  " +
                     "pc.cart_id, c.creation_time) as base " +
                     "GROUP BY " +
                     $"date_trunc('{aggregate.ToString()}', base.time)";
        
        MaterializedStatements statements = new MaterializedStatements(new MaterializedStatement(sql) {
            Parameters = new QueryParameter[] {
                new("shop_id", _tenantIdAccessor.GetTenantGuid()),
                new("from", from),
                new("until", until),
            }
        });

        return await _statementExecutor.StreamAsync(statements,
            async (record, token) => new CartStatisticsResultDateAggregate(
                await record.GetFieldValueAsync<DateTimeOffset>("cart_creation_date", token),
                await record.GetFieldValueAsync<long>("count_carts", token),
                await record.GetFieldValueAsync<decimal>("sum_value_carts", token),
                await record.GetFieldValueAsync<decimal>("avg_value_carts", token),
                await record.GetFieldValueAsync<decimal>("count_products_in_carts", token),
                await record.GetFieldValueAsync<decimal>("avg_products_in_carts", token)
            ), cancellationToken).ToListAsync(cancellationToken);
    }

}
