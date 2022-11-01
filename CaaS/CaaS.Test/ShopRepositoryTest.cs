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
    
    [Fact]
    public async Task FindAllOptimistic() {
        var shops = new List<ShopDataModel>() {
            new ShopDataModel() { Name = "ZAbc" },
            new ShopDataModel() { Name = "Amazon" },
            new ShopDataModel() { Name = "E-Tec" },
            new ShopDataModel() { Name = "360" },
            new ShopDataModel() { Name = "Yeeq" },
        };
        var shopRepository = new ShopRepository(new MemoryDao<ShopDataModel>(shops));
        var items = await shopRepository.FindAllAsync();
        items.Count.Should().Be(5);
        items[0].Name.Should().Be("360");
        items[1].Name.Should().Be("Amazon");
        items[2].Name.Should().Be("E-Tec");
        items[3].Name.Should().Be("Yeeq");
        items[4].Name.Should().Be("ZAbc");
    }
}