using CaaS.Core.Base.Pagination;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.ProductData;
using Xunit.Abstractions;

namespace CaaS.Test.Integration.DaoTests; 

public class ProductDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";

    public ProductDaoTest(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var productDao = GetProductDao(ShopTenantId);
        var products = await productDao.FindAllAsync().ToListAsync();
        products[0].Name.Should().Be("USB cable");
        products[1].Name.Should().Be("HDMI cable");
    }

    [Fact]
    public async Task FindAllWithPagination() {
        var productDao = GetProductDao(ShopTenantId);

        var statementParams = new StatementParameters() {
            OrderParameters = new OrderParameters("Name")
        };

        var paginationToken = new PaginationToken(Direction: KeysetPaginationDirection.Forward, Limit: 2);
        var pages = await productDao.FindByPagedAsync(statementParams, paginationToken);

        pages.TotalPages.Should().Be(2);
        pages.TotalCount.Should().Be(3);
        pages.NextPage!.Reference!.PropertyValues["Name"].Should().Be("LAN cable");
        pages.PreviousPage!.Reference!.PropertyValues["Name"].Should().Be("HDMI cable");
    }
    
    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var productDao = GetProductDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("fcb3c98d-4392-4e4c-8d31-f89f0ebe3c83"),
                Guid.Parse("ff66c1f5-d79e-4797-a03c-a665ae26b171")
        };
        
        var shops = await productDao.FindByIdsAsync(idList).ToListAsync();
        
        shops[0].Name.Should().Be("USB cable");
        shops[1].Name.Should().Be("HDMI cable");
    }
    
    [Fact]
    public async Task FindByNameAndPriceReturnsEntities() {
        var productDao = GetProductDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
            new(nameof(Product.Name), "HDMI cable"),
            new(nameof(Product.Price), 5.99),
        };

        var products = await productDao.FindBy(new StatementParameters { Where = parameters }).ToListAsync();
        
        products.Count.Should().NotBe(0);
        products[0].Id.Should().Be("ff66c1f5-d79e-4797-a03c-a665ae26b171");
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var productDao = GetProductDao(ShopTenantId);
        (await productDao.CountAsync()).Should().Be(3);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var productDao = GetProductDao(ShopTenantId);
        var product = new ProductDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                Name = "AddTest",
                Description = "a new product",
                DownloadLink = "download/file",
                ShopId = Guid.Parse(ShopTenantId),
                Price = 50,
                Deleted = false
        };
        await productDao.AddAsync(product);
        
        product = await productDao.FindByIdAsync(product.Id);
        product.Should().NotBeNull();
        product!.Name.Should().Be("AddTest");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var productId = Guid.Parse("fcb3c98d-4392-4e4c-8d31-f89f0ebe3c83");
        var productDao = GetProductDao(ShopTenantId);
        var product = await productDao.FindByIdAsync(productId);
        product.Should().NotBeNull();
        product!.Name.Should().Be("USB cable");
        product = product with {
                Name = "TV"
        };
        await productDao.UpdateAsync(product);
        
        product = await productDao.FindByIdAsync(productId);
        product.Should().NotBeNull();
        product!.Name.Should().Be("TV");
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var productId = Guid.Parse("587c3437-b430-405a-99dd-a0ce9ebde0a4");
        var productDao = GetProductDao(ShopTenantId);
        var product = await productDao.FindByIdAsync(productId);
        product.Should().NotBeNull();
        product!.Name.Should().Be("LAN cable");
        await productDao.DeleteAsync(product);
        
        product = await productDao.FindByIdAsync(productId);
        product.Should().BeNull();
    }
    
    [Fact]
    public async Task UpdateMultipleEntitiesBatch() {
        var productDao = GetProductDao(ShopTenantId);
        var products = await productDao.FindAllAsync()
            .Select((p, idx) => p with { Name = $"{p.Name}_{idx}" })
            .ToListAsync();
        await productDao.UpdateAsync(products);
    }

    private IDao<ProductDataModel> GetProductDao(string tenantId) => GetDao(new ProductDataRecordMapper(), tenantId);
}