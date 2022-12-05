using CaaS.Core.Base.Exceptions;
using CaaS.Core.CouponAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.CouponData;
using CaaS.Infrastructure.Gen;
using Xunit.Abstractions;

namespace CaaS.Test.Integration.DaoTests; 

public class CouponDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";

    public CouponDaoTest(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var couponDao = GetCouponDao(ShopTenantId);
        var coupons = await couponDao.FindAllAsync().ToListAsync();
        coupons[0].Id.Should().Be("bbf7b266-2485-4cd8-a0a4-822a692fab10");
        coupons[1].Id.Should().Be("aff66783-ed9c-4838-9642-72042883fffe");
    }
    
    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var orderDao = GetCouponDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("bbf7b266-2485-4cd8-a0a4-822a692fab10"),
                Guid.Parse("aff66783-ed9c-4838-9642-72042883fffe")
        };
        
        var orders = await orderDao.FindByIdsAsync(idList).ToListAsync();
        
        orders[0].Id.Should().Be("bbf7b266-2485-4cd8-a0a4-822a692fab10");
        orders[1].Id.Should().Be("aff66783-ed9c-4838-9642-72042883fffe");
    }
    
    [Fact]
    public async Task FindByValueReturnsEntities() {
        var couponDao = GetCouponDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
            new(nameof(Coupon.Value), 7)
        };

        var products = await couponDao.FindBy(new StatementParameters { Where = parameters }).ToListAsync();
        
        products.Count.Should().NotBe(0);
        products[0].Id.Should().Be("aff66783-ed9c-4838-9642-72042883fffe");
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var couponDao = GetCouponDao(ShopTenantId);
        (await couponDao.CountAsync()).Should().Be(2);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var couponDao = GetCouponDao(ShopTenantId);
        var coupon = new CouponDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ShopId = Guid.Parse(ShopTenantId),
                Value = 10,
                OrderId = null,
                CartId = null,
                CustomerId = null,
                
        };
        await couponDao.AddAsync(coupon);
        
        coupon = await couponDao.FindByIdAsync(coupon.Id);
        coupon.Should().NotBeNull();
        coupon!.Id.Should().Be("7A819343-23A1-4AD9-8798-64D1047CF01F");
        coupon!.CartId.Should().Be(null);
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var couponId = Guid.Parse("bbf7b266-2485-4cd8-a0a4-822a692fab10");
        var couponDao = GetCouponDao(ShopTenantId);
        var coupon = await couponDao.FindByIdAsync(couponId);
        coupon.Should().NotBeNull();
        coupon!.Value.Should().Be(10);
        coupon = coupon with {
                Value = 20000
        };
        await couponDao.UpdateAsync(coupon);
        
        coupon = await couponDao.FindByIdAsync(couponId);
        coupon.Should().NotBeNull();
        coupon!.Value.Should().Be(20000);
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var couponId = Guid.Parse("aff66783-ed9c-4838-9642-72042883fffe");
        var couponDao = GetCouponDao(ShopTenantId);
        var coupon = await couponDao.FindByIdAsync(couponId);
        coupon.Should().NotBeNull();
        coupon!.Id.Should().Be("aff66783-ed9c-4838-9642-72042883fffe");
        coupon = coupon with {
                Id = Guid.Parse("993bffa6-e73d-4aa1-be30-aa636fa823c0"),
                Value = 99
        };
        
        Func<Task> act = async () => { await couponDao.UpdateAsync(coupon); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var couponId = Guid.Parse("aff66783-ed9c-4838-9642-72042883fffe");
        var couponDao = GetCouponDao(ShopTenantId);
        var coupon = await couponDao.FindByIdAsync(couponId);
        coupon.Should().NotBeNull();
        coupon!.Value.Should().Be(7);
        await couponDao.DeleteAsync(coupon);
        
        coupon = await couponDao.FindByIdAsync(couponId);
        coupon.Should().BeNull();
    }
    
    private IDao<CouponDataModel> GetCouponDao(string tenantId) => GetDao(new CouponDataRecordMapper(), tenantId);
}