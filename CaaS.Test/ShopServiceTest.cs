using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test; 

public class ShopServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly Guid TestShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private const string TestShopName = "TestShop";


    private IShopService CreateShopService() {
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = TestShopAdminId, AppKey = "testKey"}
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId, ShopId = TestShopId}
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var shopAdminRepository = new ShopAdminRepository(shopAdminDao);
        var uowManager = new MockUnitOfWorkManager();

        return new ShopService(shopRepository, uowManager, shopAdminRepository);
    }

}