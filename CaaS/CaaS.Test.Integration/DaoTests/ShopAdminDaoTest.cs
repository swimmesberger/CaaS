using CaaS.Core.Entities;
using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;
using Xunit.Abstractions;

namespace CaaS.Test.Integration.DaoTests; 

public class ShopAdminDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";

    public ShopAdminDaoTest(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var shopAdminDao = GetShopAdminDao(ShopTenantId);
        var shopAdmins = await shopAdminDao.FindAllAsync().ToListAsync();
        shopAdmins[0].Name.Should().Be("Joye Killiam");
        shopAdmins[1].Name.Should().Be("Kanya Pavey");
    }
    
    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var shopAdminDao = GetShopAdminDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("bbfc1894-0c7e-4414-aa71-20d7cbbe7236"),
                Guid.Parse("a5c992d5-5065-41fe-b2d0-2fa4b5945b56")
        };
        
        var shopAdmins = await shopAdminDao.FindByIdsAsync(idList).ToListAsync();
        
        shopAdmins[0].Name.Should().Be("Joye Killiam");
        shopAdmins[1].Name.Should().Be("Kanya Pavey");
    }
    
    [Fact]
    public async Task FindByNameAndPriceReturnsEntities() {
        var shopAdminDao = GetShopAdminDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(ShopAdmin.Name), "Kanya Pavey"),
            QueryParameter.From(nameof(ShopAdmin.EMail), "kpavey0@google.ca"),
        };

        var shopAdmins = await shopAdminDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();
        
        shopAdmins.Count.Should().NotBe(0);
        shopAdmins[0].Id.Should().Be("a5c992d5-5065-41fe-b2d0-2fa4b5945b56");
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var shopAdminDao = GetShopAdminDao(ShopTenantId);
        (await shopAdminDao.CountAsync()).Should().Be(2);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var shopAdminDao = GetShopAdminDao(ShopTenantId);
        var shopAdmin = new ShopAdminDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ShopId = Guid.Parse(ShopTenantId),
                Name = "AddTest",
                EMail = "test@test.com"
        };
        await shopAdminDao.AddAsync(shopAdmin);
        
        shopAdmin = await shopAdminDao.FindByIdAsync(shopAdmin.Id);
        shopAdmin.Should().NotBeNull();
        shopAdmin!.Name.Should().Be("AddTest");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var shopAdminId = Guid.Parse("a5c992d5-5065-41fe-b2d0-2fa4b5945b56");
        var shopAdminDao = GetShopAdminDao(ShopTenantId);
        var shopAdmin = await shopAdminDao.FindByIdAsync(shopAdminId);
        shopAdmin.Should().NotBeNull();
        shopAdmin!.Name.Should().Be("Kanya Pavey");
        shopAdmin = shopAdmin with {
                Name = "test"
        };
        await shopAdminDao.UpdateAsync(shopAdmin);
        
        shopAdmin = await shopAdminDao.FindByIdAsync(shopAdminId);
        shopAdmin.Should().NotBeNull();
        shopAdmin!.Name.Should().Be("test");
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var shopAdminId = Guid.Parse("a5c992d5-5065-41fe-b2d0-2fa4b5945b56");
        var shopAdminDao = GetShopAdminDao(ShopTenantId);
        var shopAdmin = await shopAdminDao.FindByIdAsync(shopAdminId);
        shopAdmin.Should().NotBeNull();
        shopAdmin!.Id.Should().Be("a5c992d5-5065-41fe-b2d0-2fa4b5945b56");
        shopAdmin = shopAdmin with {
                Id = Guid.Parse("b5c992d5-5065-41fe-b2d0-2fa4b5945b56"),
                Name = "Whatever"
        };
        
        Func<Task> act = async () => { await shopAdminDao.UpdateAsync(shopAdmin); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var shopAdminId = Guid.Parse("a5c992d5-5065-41fe-b2d0-2fa4b5945b56");
        var shopAdminDao = GetShopAdminDao(ShopTenantId);
        var shopAdmin = await shopAdminDao.FindByIdAsync(shopAdminId);
        shopAdmin.Should().NotBeNull();
        shopAdmin!.Name.Should().Be("Kanya Pavey");
        await shopAdminDao.DeleteAsync(shopAdmin);
        
        shopAdmin = await shopAdminDao.FindByIdAsync(shopAdminId);
        shopAdmin.Should().BeNull();
    }
    
    private IDao<ShopAdminDataModel> GetShopAdminDao(string tenantId) => GetDao(new ShopAdminDataRecordMapper(), tenantId);
}