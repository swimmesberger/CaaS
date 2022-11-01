using CaaS.Infrastructure.Dao;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;

namespace CaaS.Test.Integration; 

public class ShopDaoTest : BaseDaoTest {
    [Fact]
    public async Task CountOptimistic() {
        var shopDao = GetShopDao();
        (await shopDao.CountAsync()).Should().Be(2);
    }
    
    [Fact]
    public async Task FindAllOptimistic() {
        var shopDao = GetShopDao();
        var shops = await shopDao.FindAllAsync().ToListAsync();
        shops[0].Name.Should().Be("Amazon");
        shops[1].Name.Should().Be("E-Tec");
    }
    
    [Fact]
    public async Task UpdateSingleOptimistic() {
        var shopId = Guid.Parse("AB20302A-B5ED-48C0-86F3-CB5B93E1B86F");
        var shopDao = GetShopDao();
        var shop = await shopDao.FindByIdAsync(shopId);
        shop.Should().NotBeNull();
        shop!.Name.Should().Be("Amazon");
        shop = shop with {
            Name = "Amazon2"
        };
        await shopDao.UpdateAsync(shop);
        
        shop = await shopDao.FindByIdAsync(shopId);
        shop.Should().NotBeNull();
        shop!.Name.Should().Be("Amazon2");
    }
    
    [Fact]
    public async Task DeleteSingleOptimistic() {
        var shopId = Guid.Parse("AB20302A-B5ED-48C0-86F3-CB5B93E1B86F");
        var shopDao = GetShopDao();
        var shop = await shopDao.FindByIdAsync(shopId);
        shop.Should().NotBeNull();
        shop!.Name.Should().Be("Amazon");
        await shopDao.DeleteAsync(shop);
        
        shop = await shopDao.FindByIdAsync(shopId);
        shop.Should().BeNull();
    }
    
    [Fact]
    public async Task AddSingleOptimistic() {
        var shopDao = GetShopDao();
        var shop = new ShopDataModel() {
            Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
            Name = "AddTest"
        };
        await shopDao.AddAsync(shop);
        
        shop = await shopDao.FindByIdAsync(shop.Id);
        shop.Should().NotBeNull();
        shop!.Name.Should().Be("AddTest");
    }
    
    private IDao<ShopDataModel> GetShopDao() => GetDao(new ShopDataRecordMapper());
}