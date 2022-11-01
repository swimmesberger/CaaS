using CaaS.Core.Repositories;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.Repositories;

namespace CaaS.Test.Integration; 

public class ShopRepositoryTest : BaseDaoTest {
    [Fact]
    public async Task FindAllOptimistic() {
        var shopRepository = GetShopRepository();
        var shops = await shopRepository.FindAllAsync();
        shops[0].Name.Should().Be("Amazon");
        shops[1].Name.Should().Be("E-Tec");
    }
    
    private IShopRepository GetShopRepository() => new ShopRepository(GetDao(new ShopDataRecordMapper()));
}