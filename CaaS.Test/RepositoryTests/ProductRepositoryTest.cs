using CaaS.Core.Base.Pagination;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test.RepositoryTests; 

public class ProductRepositoryTest {
    private static readonly Guid TestShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid ProductCId = new Guid("B72D37D7-47B3-4408-8853-AC55CC113352");
    private static readonly Guid ProductDId = new Guid("BA156B12-AF7C-4676-A230-7F3E17EE2176");
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private const string TestShopName = "TestShop";

    [Fact]
    public async Task FindByTextSearchOptimistic() {
        var productRepository = GetProductRepository();

        var result = await productRepository.FindByTextSearchAsync("HDMI cable", null);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task FindByIdOptimistic() {
        var productRepository = GetProductRepository();
        var result = await productRepository.FindByIdAsync(ProductAId);
        result!.Id.Should().Be(ProductAId);
    }

    [Fact]
    public async Task FindAllPaged() {
        var productRepository = GetProductRepository();
        var result = await productRepository.FindAllPagedAsync();
        var items = result.ToList();

        result.TotalPages.Should().Be(1);
        result.NextPage!.Reference!.PropertyValues["Name"].Should().Be("VGA cable");
        items.Count.Should().Be(4);
        items[0].Id.Should().Be(ProductAId);
        items[1].Id.Should().Be(ProductDId);
        items[2].Id.Should().Be(ProductBId);
        items[3].Id.Should().Be(ProductCId);
    }
    
    
    private IProductRepository GetProductRepository() {
        var productDao = new MemoryDao<ProductDataModel>(new List<ProductDataModel>() {
            new ProductDataModel() { Id = ProductAId, Name = "HDMI cable", Price = 10, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductBId, Name = "USB cable", Price = 20, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductCId, Name = "VGA cable", Price = 5, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductDId, Name = "LED lights", Price = 7, ShopId = TestShopId }
        });
        
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = TestShopAdminId }
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId, ShopId = TestShopId }
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        
        var productRepository = new ProductRepository(productDao, shopRepository);
        return productRepository;
    }
    
    }