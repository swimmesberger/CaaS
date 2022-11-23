using System.Collections.Immutable;
using System.Text.Json;
using CaaS.Core;
using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.DiscountData;
using CaaS.Test.Common;
using Microsoft.Extensions.DependencyInjection;

namespace CaaS.Test; 

public class DiscountServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    
    [Fact]
    public async Task ApplyDiscountTimeOutOfRange() {
        // out of range date
        DateTimeOffsetProvider.Instance = new StaticDateTimeOffsetProvider(AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local)));
        
        await using var serviceProvider = SetupDiscountServiceProvider();
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();
        var cart = CreateTestCart();
        cart = await discountService.ApplyDiscountAsync(cart);
        cart.CartDiscounts.Should().HaveCount(0);
        cart.Items.Should().AllSatisfy(i => i.CartItemDiscounts.Should().HaveCount(0));
    }
    
    [Fact]
    public async Task ApplyDiscountTimeInRange() {
        // in range date
        DateTimeOffsetProvider.Instance = new StaticDateTimeOffsetProvider(AsUtc(new DateTime(2022, 11, 25, 0, 0, 0, DateTimeKind.Local)));
        
        await using var serviceProvider = SetupDiscountServiceProvider();
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();
        var cart = CreateTestCart();
        cart = await discountService.ApplyDiscountAsync(cart);
        cart.CartDiscounts.Should().HaveCount(1);
        cart.CartDiscounts[0].DiscountValue.Should().Be(49.94m);
        cart.Items.Should().AllSatisfy(i => i.CartItemDiscounts.Should().HaveCount(0));
    }

    private ServiceProvider SetupDiscountServiceProvider() {
        var discountSettingsDao = new MemoryDao<DiscountSettingDataModel>(new List<DiscountSettingDataModel>() {
            new DiscountSettingDataModel() {
                ShopId = TestShopId, 
                Name = "Black Friday", 
                RuleId = TimeWindowDiscountRule.Id, 
                RuleParameters = JsonSerializer.SerializeToElement(new TimeWindowDiscountSettings() {
                    FromTime = AsUtc(new DateTime(2022, 11, 25, 0, 0, 0, DateTimeKind.Local)),
                    ToTime = AsUtc(new DateTime(2022, 11, 26, 0, 0, 0, DateTimeKind.Local))
                }),
                ActionId = PercentageDiscountAction.Id,
                ActionParameters = JsonSerializer.SerializeToElement(new PercentageDiscountSettings() {
                    Percentage = 0.25m
                }),
            }
        });
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IDao<DiscountSettingDataModel>>(_ => discountSettingsDao);
        serviceCollection.AddScoped<IDiscountSettingRepository, DiscountSettingsRepository>();
        serviceCollection.AddScoped(_ => new JsonSerializerOptions());
        serviceCollection.AddCaasDiscountCore();

        return serviceCollection.BuildServiceProvider();
    }

    private Cart CreateTestCart() {
        var productA = new Product() {
            Name = "USB cable",
            Price = 12.99m
        };
        
        var productB = new Product() {
            Name = "HDMI cable",
            Price = 22.97m
        };

        var cart = new Cart() {
            ShopId = TestShopId,
            Items = new[] {
                new CartItem(){ Product = productA , Amount = 3 },
                new CartItem(){ Product = productB , Amount = 7 }
            }.ToImmutableArray()
        };
        return cart;
    }

    private static DateTimeOffset AsUtc(DateTime dateTime) => new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime));
}