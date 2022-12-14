using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test; 

public class ShopServiceTest {
    private static readonly Guid TestShopId1 = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly Guid TestShopId2 = new Guid("785926E5-8E69-4525-AB2A-6245E415F102");
    private static readonly Guid TestShopAdminId1 = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private static readonly Guid TestShopAdminId2 = new Guid("829A196B-5ACF-44F5-B013-51EFFDFA754E");
    private const string TestShopName1 = "Amazon";
    private const string TestShopName2 = "MediaMarkt";

    [Fact]
    public async Task GetByNameOptimistic() {
        var shopService = CreateShopService();

        var shop = await shopService.GetByName(TestShopName1
        );
        shop!.Id.Should().Be(TestShopId1);
    }

    [Fact]
    public async Task GetAll() {
        var shopService = CreateShopService();
        var shops = await shopService.GetAll();
        shops.TotalCount.Should().Be(2);
        var shopItems = shops.Select(s => s).ToArray();
        shopItems[0].Id.Should().Be(TestShopId1);
    }

    [Fact]
    public async Task SetName() {
        var shopService = CreateShopService();
        var updatedShop = await shopService.SetName(TestShopId1, "Aaaamazon");
        var shop = await shopService.GetByName("Aaaamazon");
        shop!.Name.Should().Be("Aaaamazon");
    }

    [Fact]
    public async Task AddShopOptimistic() {
        var shopService = CreateShopService();
        await shopService.Add(new Shop {
            Id = Guid.Parse("CE005B43-4C62-4BF9-9045-1AA48F9170CA"),
            Name = "added shop",
            CartLifetimeMinutes = 120,
            ShopAdmin = new ShopAdmin {
                Id = TestShopAdminId1
            },
            AppKey = "no key",
        });

        var allShops = await shopService.GetAll();
        allShops.TotalCount.Should().Be(3);
        var shops = allShops.Select(s => s).ToArray();
        shops[0].Id.Should().Be("CE005B43-4C62-4BF9-9045-1AA48F9170CA");
    }

    private IShopService CreateShopService() {
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId1, Name = TestShopName1, AdminId = TestShopAdminId1, AppKey = "testKey"},
            new ShopDataModel() { Id = TestShopId2, Name = TestShopName2, AdminId = TestShopAdminId2, AppKey = "testKey"}
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId1, ShopId = TestShopId1},
            new ShopAdminDataModel() { Id = TestShopAdminId2, ShopId = TestShopId2}
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var shopAdminRepository = new ShopAdminRepository(shopAdminDao);
        var uowManager = new MockUnitOfWorkManager();

        return new ShopService(shopRepository, uowManager, shopAdminRepository);
    }

}