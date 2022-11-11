using System.Collections.Immutable;
using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;
using Xunit.Abstractions;

namespace CaaS.Test.Integration.DaoTests; 

public class DiscountSettingDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";
    
    public DiscountSettingDaoTest(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var discountSettingDao = GetDiscountSettingDao(ShopTenantId);
        var discountSettings = await discountSettingDao.FindAllAsync().ToListAsync();
        discountSettings[0].Id.Should().Be("7981f1dc-76d4-4d7b-b823-af2e841d8001");
        discountSettings[1].Id.Should().Be("b04bf20d-0abf-47c4-94d9-185536df9867");
    }
    
    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var discountSettingDao = GetDiscountSettingDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("7981f1dc-76d4-4d7b-b823-af2e841d8001"),
                Guid.Parse("b04bf20d-0abf-47c4-94d9-185536df9867")
        };
        
        var discountSettings = await discountSettingDao.FindByIdsAsync(idList).ToListAsync();
        
        discountSettings[0].Id.Should().Be("7981f1dc-76d4-4d7b-b823-af2e841d8001");
        discountSettings[1].Id.Should().Be("b04bf20d-0abf-47c4-94d9-185536df9867");
    }
    
    [Fact]
    public async Task FindByNameReturnsEntities() {
        var discountSettingDao = GetDiscountSettingDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(DiscountSettingDataModel.Name), "Christmas2022")
        };

        var discountSettings = await discountSettingDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();
        
        discountSettings.Count.Should().NotBe(0);
        discountSettings[0].Id.Should().Be("7981f1dc-76d4-4d7b-b823-af2e841d8001");
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var discountSettingDao = GetDiscountSettingDao(ShopTenantId);
        (await discountSettingDao.CountAsync()).Should().Be(3);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var discountSettingDao = GetDiscountSettingDao(ShopTenantId);
        var discountSetting = new DiscountSettingDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ShopId = Guid.Parse(ShopTenantId),
                Name = "Krampus",
                Rule = Guid.Parse("4904887e-7761-432d-a36e-efc112246818"),
                Action = Guid.Parse("934cf35e-8875-4984-b942-ff6ec2f2df6b"),
                RuleParameters = CreateParameters("Rule", 3),
                ActionParameters = CreateParameters("Action", 7)
        };
        await discountSettingDao.AddAsync(discountSetting);
        
        discountSetting = await discountSettingDao.FindByIdAsync(discountSetting.Id);
        discountSetting.Should().NotBeNull();
        discountSetting!.Id.Should().Be("7A819343-23A1-4AD9-8798-64D1047CF01F");
        discountSetting.Name.Should().Be("Krampus");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var discountSettingId = Guid.Parse("2f87ca9b-9829-437e-b78b-f71dcc2de7a0");
        var discountSettingDao = GetDiscountSettingDao(ShopTenantId);
        var discountSetting = await discountSettingDao.FindByIdAsync(discountSettingId);
        discountSetting.Should().NotBeNull();
        discountSetting!.Name.Should().Be("Valentines day");
        discountSetting = discountSetting with {
                Name = "Dooms day"
        };
        await discountSettingDao.UpdateAsync(discountSetting);
        
        discountSetting = await discountSettingDao.FindByIdAsync(discountSettingId);
        discountSetting.Should().NotBeNull();
        discountSetting!.Name.Should().Be("Dooms day");
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var discountSettingId = Guid.Parse("2f87ca9b-9829-437e-b78b-f71dcc2de7a0");
        var discountSettingDao = GetDiscountSettingDao(ShopTenantId);
        var discountSetting = await discountSettingDao.FindByIdAsync(discountSettingId);
        discountSetting.Should().NotBeNull();
        discountSetting!.Id.Should().Be("2f87ca9b-9829-437e-b78b-f71dcc2de7a0");
        discountSetting = discountSetting with {
                Id = Guid.Parse("9f87ca9b-9829-437e-b78b-f71dcc2de7a0"),
                Name = "Test"
        };
        
        Func<Task> act = async () => { await discountSettingDao.UpdateAsync(discountSetting); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var discountSettingId = Guid.Parse("2f87ca9b-9829-437e-b78b-f71dcc2de7a0");
        var discountSettingDao = GetDiscountSettingDao(ShopTenantId);
        var discountSetting = await discountSettingDao.FindByIdAsync(discountSettingId);
        discountSetting.Should().NotBeNull();
        discountSetting!.Name.Should().Be("Valentines day");
        await discountSettingDao.DeleteAsync(discountSetting);
        
        discountSetting = await discountSettingDao.FindByIdAsync(discountSettingId);
        discountSetting.Should().BeNull();
    }
    
    private IDao<DiscountSettingDataModel> GetDiscountSettingDao(string tenantId) => GetDao(new DiscountSettingDataRecordMapper(), tenantId);

    private static ImmutableDictionary<string, object> CreateParameters(string key, object value) {
        var dict = ImmutableDictionary.CreateBuilder<string, object>();
        dict.Add(key, value);
        return dict.ToImmutable();
    }
}