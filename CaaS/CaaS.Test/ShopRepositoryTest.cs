using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories;

namespace CaaS.Test; 

public class ShopRepositoryTest {
    [Fact]
    public async Task CountOptimistic() {
        var shops = new List<ShopDataModel>() {
            new ShopDataModel(),
            new ShopDataModel(),
            new ShopDataModel(),
            new ShopDataModel(),
            new ShopDataModel(),
        };
        var shopRepository = new ShopRepository(new MemoryDao<ShopDataModel>(shops));
        (await shopRepository.CountAsync()).Should().Be(5);
    }
}