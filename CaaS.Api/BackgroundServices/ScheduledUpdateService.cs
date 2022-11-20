using CaaS.Core.CartAggregate;
using CaaS.Core.ShopAggregate;

namespace CaaS.Api.BackgroundServices; 

public class ScheduledUpdateService : BackgroundService {
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(20);
    
    public ScheduledUpdateService(IServiceProvider serviceProvider, ILogger<ScheduledUpdateService> logger) {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            using (var scope = _serviceProvider.CreateScope()) {
                var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
                await cartService.DeleteExpiredCarts(stoppingToken);
                _logger.LogInformation("scheduled update is conducted");
            }
            await Task.Delay(UpdateInterval, stoppingToken);
        }
    }
}