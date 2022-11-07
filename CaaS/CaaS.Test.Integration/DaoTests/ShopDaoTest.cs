﻿using CaaS.Core.Entities;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;

namespace CaaS.Test.Integration.DaoTests; 

public class ShopDaoTest : BaseDaoTest {
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var shopDao = GetShopDao();
        var shops = await shopDao.FindAllAsync().ToListAsync();
        shops[0].Name.Should().Be("Amazon");
        shops[1].Name.Should().Be("E-Tec");
    }

    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var shopDao = GetShopDao();
        var idList = new List<Guid> {
                Guid.Parse("a468d796-db09-496d-9794-f6b42f8c7c0b"),
                Guid.Parse("ba86a395-51ca-4f29-a047-5296ce90ab79")
        };
        
        var shops = await shopDao.FindByIdsAsync(idList).ToListAsync();
        
        shops[0].Name.Should().Be("Amazon");
        shops[1].Name.Should().Be("E-Tec");
    }

    [Fact]
    public async Task FindByCartLifetimeMinutesAndNameReturnsEntities() {
        var shopDao = GetShopDao();

        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(Shop.CartLifetimeMinutes), 44415),
            QueryParameter.From(nameof(Shop.Name), "Amazon"),
        };

        var shops = await shopDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();

        shops.Count.Should().NotBe(0);
        shops[0].Name.Should().Be("Amazon");
        shops[0].CartLifetimeMinutes.Should().Be(44415);
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var shopDao = GetShopDao();
        (await shopDao.CountAsync()).Should().Be(3);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var shopDao = GetShopDao();
        var shop = new ShopDataModel() {
            Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
            Name = "AddTest",
            CartLifetimeMinutes = 3000,
            AdminId = Guid.Parse("bbfc1894-0c7e-4414-aa71-20d7cbbe7236")
        };
        await shopDao.AddAsync(shop);
        
        shop = await shopDao.FindByIdAsync(shop.Id);
        shop.Should().NotBeNull();
        shop!.Name.Should().Be("AddTest");
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var shopId = Guid.Parse("a468d796-db09-496d-9794-f6b42f8c7c0b");
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
    public async Task DeleteEntityWhenEntityIsExisting() {
        var shopId = Guid.Parse("c277a395-51ca-4f29-a047-5296ce90ab79");
        var shopDao = GetShopDao();
        var shop = await shopDao.FindByIdAsync(shopId);
        shop.Should().NotBeNull();
        shop!.Name.Should().Be("Kanoodle");
        await shopDao.DeleteAsync(shop);
        
        shop = await shopDao.FindByIdAsync(shopId);
        shop.Should().BeNull();
    }
    
    private IDao<ShopDataModel> GetShopDao() => GetDao(new ShopDataRecordMapper());
}