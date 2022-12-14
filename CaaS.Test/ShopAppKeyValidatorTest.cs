using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test; 

public class ShopAppKeyValidatorTest {
    private static readonly Guid TestShopId1 = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly Guid TestShopAdminId1 = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private const string TestShopName1 = "Amazon";

    [Fact]
    public async Task ValidateAppKeyAsyncOptimistic() {
        var validator = CreateShopAppKeyValidator();
        var result = await validator.ValidateAppKeyAsync("testKey");
        result.Should().Be(true);
    }
    
    [Fact]
    public async Task ValidateAppKeyAsyncPessimistic() {
        var validator = CreateShopAppKeyValidator();
        var result = await validator.ValidateAppKeyAsync("lalala");
        result.Should().Be(false);
    }
    
    private IAppKeyValidator CreateShopAppKeyValidator() {
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId1, Name = TestShopName1, AdminId = TestShopAdminId1, AppKey = "testKey"}
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId1, ShopId = TestShopId1},
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var tenant = new StaticTenantIdAccessor(TestShopId1.ToString());

        return new ShopAppKeyValidator(shopRepository, tenant);
    }
}