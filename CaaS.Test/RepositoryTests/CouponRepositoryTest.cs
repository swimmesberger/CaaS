using CaaS.Core.CouponAggregate;
using CaaS.Infrastructure.CouponData;
using CaaS.Test.Common;

namespace CaaS.Test.RepositoryTests; 

public class CouponRepositoryTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly string TestShopName = "TestShop";
    private static readonly Guid ExistingCart1Id = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ExistingOrderId = new Guid("AAABBB4E-E2DF-4A0E-B055-C804A6672555");
    private static readonly Guid ExistingOrder2Id = new Guid("C93FD5D8-3BEB-48FB-B0D6-5BE6312D2A45");
    private static readonly Guid CustomerIdA = new Guid("99C91EA1-4CA5-9097-DFDB-CF688F0DA31F");
    private static readonly Guid CustomerIdB = new Guid("9C0E8AA6-94C6-43F2-8446-D496DED2D7FE");
    private static readonly Guid CouponIdA = new Guid("BB791EA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid CouponIdB = new Guid("4276016B-2BE5-41C7-994A-1486257605CB");
    private static readonly Guid CouponIdC = new Guid("B5215C84-7419-4D22-B224-F24F0E28767D");
    private static readonly Guid CouponIdD = new Guid("7986CE25-2E4F-4F44-8ABF-7FA68ADF152E");

    [Fact]
    public async Task FindByCustomerId() {
        var couponRepository = CreateCouponRepository();
        var result = await couponRepository.FindByCustomerId(CustomerIdA);
        result.Count().Should().Be(2);
        result[0].Id.Should().Be(CouponIdA);
        result[1].Id.Should().Be(CouponIdB);
    }

    [Fact]
    public async Task FindByOrderId() {
        var couponRepository = CreateCouponRepository();
        var result = await couponRepository.FindByOrderId(ExistingOrderId);
        result.Count().Should().Be(2);
        result[0].Id.Should().Be(CouponIdC);
        result[1].Id.Should().Be(CouponIdD);
    }
    
    [Fact]
    public async Task FindByOrderIds() {
        var couponRepository = CreateCouponRepository();
        var orderList = new List<Guid>() { ExistingOrderId, ExistingOrder2Id };
        var result = await couponRepository.FindByOrderIds(orderList);
        result.Count().Should().Be(2);
        result.Should().ContainKey(ExistingOrderId);
        result.Should().ContainKey(ExistingOrder2Id);
        result[ExistingOrderId][0].Id.Should().Be(CouponIdC);
        result[ExistingOrderId][1].Id.Should().Be(CouponIdD);
        result[ExistingOrder2Id][0].Id.Should().Be(CouponIdA);
        result[ExistingOrder2Id][1].Id.Should().Be(CouponIdB);
    }
    
    [Fact]
    public async Task FindByCartId() {
        var couponRepository = CreateCouponRepository();
        var result = await couponRepository.FindByCartId(ExistingCart1Id);
        result.Count().Should().Be(2);
        result[0].Id.Should().Be(CouponIdA);
        result[1].Id.Should().Be(CouponIdB);
    }
    
    [Fact]
    public async Task FindByCartIds() {
        var couponRepository = CreateCouponRepository();
        var cartList = new List<Guid>() { ExistingCart1Id};
        var result = await couponRepository.FindByCartIds(cartList);
        result.Count().Should().Be(1);
        result.Should().ContainKey(ExistingCart1Id);
        result[ExistingCart1Id][0].Id.Should().Be(CouponIdA);
        result[ExistingCart1Id][1].Id.Should().Be(CouponIdB);
    }
    
    private ICouponRepository CreateCouponRepository() {
        var couponDao = new MemoryDao<CouponDataModel>(new List<CouponDataModel>() {
            new CouponDataModel { Id = CouponIdA, ShopId = TestShopId, Value = 4, OrderId = ExistingOrder2Id, CartId = ExistingCart1Id, CustomerId = CustomerIdA},
            new CouponDataModel { Id = CouponIdB, ShopId = TestShopId, Value = 10, OrderId = ExistingOrder2Id, CartId = ExistingCart1Id, CustomerId = CustomerIdA},
            new CouponDataModel { Id = CouponIdC, ShopId = TestShopId, Value = 30, OrderId = ExistingOrderId, CartId = null, CustomerId = CustomerIdB},
            new CouponDataModel { Id = CouponIdD, ShopId = TestShopId, Value = 20, OrderId = ExistingOrderId, CartId = null, CustomerId = CustomerIdB},
        });
        
        return new CouponRepository(couponDao);
    }
}