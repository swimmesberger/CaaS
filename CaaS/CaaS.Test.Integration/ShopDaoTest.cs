using CaaS.Infrastructure.Gen;

namespace CaaS.Test.Integration; 

public class ShopDaoTest : BaseDaoTest {
    [Fact]
    public async Task CountOptimistic() {
        var shopDao = GetDao(new ShopDataRecordMapper());
        (await shopDao.CountAsync()).Should().Be(2);
    }
    
    [Fact]
    public async Task FindAll() {
        var shopDao = GetDao(new ShopDataRecordMapper());
        var shops = await shopDao.FindAllAsync().ToListAsync();
        shops[0].Name.Should().Be("Amazon");
        shops[1].Name.Should().Be("E-Tec");
    }
}