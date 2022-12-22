using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test.RepositoryTests;

public class ShopRepositoryTest {
    private static readonly Guid TestShopId1 = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly Guid TestShopId2 = new Guid("785926E5-8E69-4525-AB2A-6245E415F102");
    private static readonly Guid TestShopAdminId1 = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private static readonly Guid TestShopAdminId2 = new Guid("829A196B-5ACF-44F5-B013-51EFFDFA754E");
    private const string TestShopName1 = "Amazon";
    private const string TestShopName2 = "MediaMarkt";
    private IShopRepository CreateShopService() {
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId1, Name = TestShopName1, AdminId = TestShopAdminId1, AppKey = "testKey", CartLifetimeMinutes = 400},
            new ShopDataModel() { Id = TestShopId2, Name = TestShopName2, AdminId = TestShopAdminId2, AppKey = "testKey2", CartLifetimeMinutes = 1000}
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId1, ShopId = TestShopId1},
            new ShopAdminDataModel() { Id = TestShopAdminId2, ShopId = TestShopId2}
        });
        
        return new ShopRepository(shopDao, shopAdminDao);
    }
    
    [Fact]
    public async Task CountOptimistic() {
        var shopRepository = CreateShopService();
        (await shopRepository.CountAsync()).Should().Be(2);
    }
    
    [Fact]
    public async Task FindByAdminIdOptimistic() {
        var shopRepository = CreateShopService();
        var shop = await shopRepository.FindByAdminIdAsync(TestShopAdminId1);
        shop!.Id.Should().Be(TestShopId1);
    }

    [Fact]
    public async Task FindAllOptimistic() {
        var shopRepository = CreateShopService();
        var items = await shopRepository.FindAllAsync();
        items.Count.Should().Be(2);
        items[0].Name.Should().Be("Amazon");
        items[1].Name.Should().Be("MediaMarkt");
    }

    [Fact]
    public async Task FindAllIdsOptimistic() {
        var shopRepository = CreateShopService();
        var ids = await shopRepository.FindAllIdsAsync();
        ids[0].Should().Be(TestShopId1);
        ids[1].Should().Be(TestShopId2);
    }

    [Fact]
    public async Task FindByIdOptimistic() {
        var shopRepository = CreateShopService();
        var shop = await shopRepository.FindByIdAsync(TestShopId1);
        shop!.Id.Should().Be(TestShopId1);
    }

    [Fact]
    public async Task VerifyAppKeyOptimistic() {
        var shopRepository = CreateShopService();
        var valid = await shopRepository.VerifyAppKeyAsync(TestShopId1, "testKey");
        valid.Should().Be(true);
    }
    
    [Fact]
    public async Task VerifyAppKeyPessimistic() {
        var shopRepository = CreateShopService();
        var valid = await shopRepository.VerifyAppKeyAsync(TestShopId1, "lalala");
        valid.Should().Be(false);
    }

    [Fact]
    public async Task FindByNameOptimistic() {
        var shopRepository = CreateShopService();
        var shop = await shopRepository.FindByNameAsync("Amazon");
        shop!.Id.Should().Be(TestShopId1);
    }
    
    [Fact]
    public async Task FindByNamePessimistic() {
        var shopRepository = CreateShopService();
        var shop = await shopRepository.FindByNameAsync("lala");
        shop.Should().BeNull();
    }

    [Fact]
    public async Task AddOptimistic() {
        var shopRepository = CreateShopService();
        var newShop = new Shop {
            Id = Guid.Parse("B66BC15E-F0C4-4360-83FD-BA86D12A6465"),
            Name = "new shop",
            CartLifetimeMinutes = 500,
            ShopAdmin = new ShopAdmin() {
                Id = TestShopAdminId1
            },
            AppKey = "newKey"
        };

        var result = await shopRepository.AddAsync(newShop);
        result.Id.Should().Be(Guid.Parse("B66BC15E-F0C4-4360-83FD-BA86D12A6465"));
        var count = await shopRepository.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task UpdateOptimistic() {
        var shopRepository = CreateShopService();
        var shop = await shopRepository.FindByIdAsync(TestShopId1);
        var updatedShop = shop! with {
            CartLifetimeMinutes = 799
        };
        
        var result = await shopRepository.UpdateAsync(shop, updatedShop);
        result.CartLifetimeMinutes.Should().Be(799);
    }

    [Fact]
    public async Task FindCartLifetimeByIdAsyncOptimistic() {
        var shopRepository = CreateShopService();
        var lifetime = await shopRepository.FindCartLifetimeByIdAsync(TestShopId1);
        lifetime!.Should().Be(400);
    }
    
    [Fact]
    public async Task FindCartLifetimeByIdAsyncPessimistic() {
        var shopRepository = CreateShopService();
        var lifetime = await shopRepository.FindCartLifetimeByIdAsync(Guid.Parse("BA49149F-3B0E-4C07-A917-D8426F2F872A"));
        lifetime.Should().BeNull();
    }
}