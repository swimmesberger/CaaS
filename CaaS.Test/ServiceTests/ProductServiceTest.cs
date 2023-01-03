using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Url;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.BlobData;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test.ServiceTests; 

public class ProductServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly string TestShopName = "TestShop";
    private static readonly Guid TestShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid ProductCId = new Guid("4F9EBA78-26B2-4740-91D9-9A3BCA6A5AB7");
    private static readonly Guid ProductDId = new Guid("23466CB7-C6C7-4B9C-AF95-8F348A9B4CD0");

    [Fact]
    public async Task GetByTextSearchOptimistic() {
        var productService = CreateProductService();
        var result = await productService.GetByTextSearchAsync("nice");
        result.Items.Count.Should().Be(3);
        result.TotalPages.Should().Be(1);
        var items = result.Items.ToArray();
        items[0].Name.Should().Be("ProductA");
        items[1].Name.Should().Be("ProductB");
        items[2].Name.Should().Be("ProductC");
    }

    [Fact]
    public async Task AddProductOptimistic() {
        var productId = Guid.Parse("6E8BDD5F-DF93-438F-871E-EF7FB57882CE");
        var product = new Product {
            Id = productId,
            Shop = new Shop() {
                Id = TestShopId
            },
            Name = "the new product",
            Description = "very good",
            DownloadLink = "here",
            Price = 55,
            Deleted = false
        };

        var productService = CreateProductService();
        var result = await productService.AddAsync(product);
        result.Id.Should().Be(productId);
        result.Name.Should().Be("the new product");
    }

    [Fact]
    public async Task UpdateOptimistic() {
        var productService = CreateProductService();
        var oldProduct = await productService.GetByIdAsync(ProductAId);
        oldProduct!.Name.Should().Be("ProductA");

        var updatedProduct = oldProduct with {
            Name = "Super duper product"
        };

        var result = await productService.UpdateAsync(updatedProduct);
        result.Name.Should().Be("Super duper product");

        result = await productService.GetByIdAsync(ProductAId);
        result!.Name.Should().Be("Super duper product");
    }

    [Fact]
    public async Task SetPriceOptimistic() {
        var productService = CreateProductService();
        var product = await productService.GetByIdAsync(ProductAId);
        product!.Price.Should().Be(10);
        var updatedProduct = product with {
            Price = 99
        };

        product = await productService.UpdateAsync(updatedProduct);
        product.Price.Should().Be(99);
    }
    
    [Fact]
    public async Task SetPriceInvalidPrice() {
        var productService = CreateProductService();
        var product = await productService.GetByIdAsync(ProductAId);
        product!.Price.Should().Be(10);
        var updatedProduct = product with {
            Price = -1
        };
        
        Func<Task> act = async () => { await productService.UpdateAsync(updatedProduct); };
        await act.Should().ThrowAsync<CaasValidationException>();
    }
    
    [Fact]
    public async Task SetPriceInvalidProduct() {
        var productService = CreateProductService();
        var updatedProduct = new Product() {
            Id = Guid.Parse("BE16A432-EDD8-4DDB-8D06-7F53C3DBA7B1"),
            Price = 100
        };

        Func<Task> act = async () => { await productService.UpdateAsync(updatedProduct); };
        await act.Should().ThrowAsync<CaasItemNotFoundException>();
    }

    [Fact]
    public async Task DeleteProductOptimistic() {
        var productService = CreateProductService();
        var product = await productService.GetByIdAsync(ProductAId);
        product!.Deleted.Should().Be(false);
        await productService.DeleteAsync(ProductAId);
        product = await productService.GetByIdAsync(ProductAId);
        product!.Deleted.Should().Be(true);
    }
    
    [Fact]
    public async Task DeleteProductPessimistic() {
        var productService = CreateProductService();
        Func<Task> act = async () => { await productService.DeleteAsync(Guid.Parse("BE16A432-EDD8-4DDB-8D06-7F53C3DBA7B1")); };
        await act.Should().ThrowAsync<CaasItemNotFoundException>();
    }
    

    private IProductService CreateProductService() {
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = TestShopAdminId }
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId, ShopId = TestShopId }
        });
        var productDao = new MemoryDao<ProductDataModel>(new List<ProductDataModel>() {
            new ProductDataModel() { Id = ProductAId, Name = "ProductA", Description = "very very nice", Price = 10, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductBId, Name = "ProductB", Description = "also nice", Price = 20, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductCId, Name = "ProductC", Description = "a little bit less, but still nice", Price = 30, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductDId, Name = "ProductD", Description = "very ugly", Price = 40, ShopId = TestShopId }
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var productRepository = new ProductRepository(productDao, shopRepository, NoOpLinkGenerator.Instance);
        var blobService = new BlobService(new MemoryDao<BlobDataModel>());
        var uowManager = new MockUnitOfWorkManager();
        var tenantIdAccessor = new StaticTenantIdAccessor(TestShopId.ToString());

        return new ProductService(productRepository, shopRepository, blobService, uowManager, tenantIdAccessor);
    }
}