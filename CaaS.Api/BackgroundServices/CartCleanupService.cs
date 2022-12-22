using CaaS.Core.CartAggregate;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Tenant;

namespace CaaS.Api.BackgroundServices; 

public class CartCleanupService : BackgroundService {
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(20);
    
    public CartCleanupService(ILogger<CartCleanupService> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            _logger.LogInformation("CartCleanupService starting");
            using (var scope = _serviceProvider.CreateScope()) {
                var tenantIdAccessor = scope.ServiceProvider.GetRequiredService<StaticTenantIdAccessor>();
                var shopRepository = scope.ServiceProvider.GetRequiredService<IShopRepository>();
                var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
                foreach (var shopId in await shopRepository.FindAllIdsAsync(stoppingToken)) {
                    _logger.LogInformation("Checking for expired cards for shop {ShopId}", shopId);
                    // set the tenant id for the current scope
                    tenantIdAccessor.SetTenantGuid(shopId);
                    // delete expired carts
                    var removedCartCount = await cartService.DeleteExpiredCartsAsync(stoppingToken);
                    _logger.LogInformation("Removed {CartCount} expired carts for shop {ShopId}", removedCartCount, shopId);
                }
            }
            _logger.LogInformation("CartCleanupService finished");
            await Task.Delay(UpdateInterval, stoppingToken);
        }
    }
}