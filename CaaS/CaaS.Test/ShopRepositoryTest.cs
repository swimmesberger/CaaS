using System.Collections.Immutable;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories;
using CaaS.Test.Base;

namespace CaaS.Test;

public class ShopRepositoryTest {
    private static readonly ImmutableList<ShopAdminDataModel> ShopAdmins = ImmutableList.CreateRange(new List<ShopAdminDataModel>() {
        new ShopAdminDataModel(),
        new ShopAdminDataModel(),
        new ShopAdminDataModel(),
        new ShopAdminDataModel(),
        new ShopAdminDataModel()
    });

    [Fact]
    public async Task CountOptimistic() {
        var shops = new List<ShopDataModel>() {
            new ShopDataModel(),
            new ShopDataModel(),
            new ShopDataModel(),
            new ShopDataModel(),
            new ShopDataModel(),
        };
        var shopRepository = new ShopRepository(
            new MemoryDao<ShopDataModel>(shops),
            new MemoryDao<ShopAdminDataModel>(ShopAdmins.ToList())
        );
        (await shopRepository.CountAsync()).Should().Be(5);
    }

    [Fact]
    public async Task FindAllOptimistic() {
        var shops = new List<ShopDataModel>() {
            new ShopDataModel() { Name = "ZAbc", AdminId = ShopAdmins[0].Id },
            new ShopDataModel() { Name = "Amazon", AdminId = ShopAdmins[0].Id },
            new ShopDataModel() { Name = "E-Tec", AdminId = ShopAdmins[0].Id },
            new ShopDataModel() { Name = "360", AdminId = ShopAdmins[0].Id },
            new ShopDataModel() { Name = "Yeeq", AdminId = ShopAdmins[0].Id },
        };
        var shopRepository = new ShopRepository(
            new MemoryDao<ShopDataModel>(shops),
            new MemoryDao<ShopAdminDataModel>(ShopAdmins.ToList())
        );
        var items = await shopRepository.FindAllAsync();
        items.Count.Should().Be(5);
        items[0].Name.Should().Be("360");
        items[1].Name.Should().Be("Amazon");
        items[2].Name.Should().Be("E-Tec");
        items[3].Name.Should().Be("Yeeq");
        items[4].Name.Should().Be("ZAbc");
    }
}