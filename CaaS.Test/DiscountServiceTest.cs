using System.Collections.Immutable;
using CaaS.Core;
using CaaS.Core.Base;
using CaaS.Core.Base.Tenant;
using CaaS.Core.Base.Validation;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.DiscountData;
using CaaS.Test.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CaaS.Test;

public class DiscountServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly Guid UsbCableProductId = new Guid("3BC89C18-279B-4F91-BEE1-BDA6F0C0DC13");
    private static readonly Guid HdmiCableProductId = new Guid("2F850498-45E8-4802-9B3E-30517EBD2911");

    [Fact]
    public async Task ApplyDiscountTimeOutOfRange() {
        // out of range date
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        await using var serviceProvider = SetupDiscountServiceProvider(currentTime, CreateBlackFridaySettings);
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();
        var cart = CreateTestCart();
        cart = await discountService.ApplyDiscountAsync(cart);
        cart.CartDiscounts.Should().HaveCount(0);
        cart.Items.Should().AllSatisfy(i => i.CartItemDiscounts.Should().HaveCount(0));
    }

    [Fact]
    public async Task ApplyDiscountTimeInRange() {
        // in range date
        var currentTime = AsUtc(new DateTime(2022, 11, 25, 0, 0, 0, DateTimeKind.Local));
        await using var serviceProvider = SetupDiscountServiceProvider(currentTime, CreateBlackFridaySettings);
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();
        var cart = CreateTestCart();
        cart = await discountService.ApplyDiscountAsync(cart);
        cart.CartDiscounts.Should().HaveCount(1);
        cart.CartDiscounts[0].DiscountValue.Should().Be(49.94m);
        cart.Items.Should().AllSatisfy(i => i.CartItemDiscounts.Should().HaveCount(0));
    }

    [Fact]
    public async Task ApplyDiscountTimedSingle() {
        // in range date
        var currentTime = AsUtc(new DateTime(2022, 11, 25, 0, 0, 0, DateTimeKind.Local));

        await using var serviceProvider = SetupDiscountServiceProvider(currentTime, CreateBlackFridaySettings);
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();
        var cart = CreateTestCart();
        cart = await discountService.ApplyDiscountAsync(cart);
        cart.CartDiscounts.Should().HaveCount(1);
        cart.CartDiscounts[0].DiscountValue.Should().Be(49.94m);
        cart.Items.Should().AllSatisfy(i => i.CartItemDiscounts.Should().HaveCount(0));
    }

    [Fact]
    public async Task ApplyDiscountMulti() {
        // in range date
        var currentTime = AsUtc(new DateTime(2023, 02, 14, 0, 0, 0, DateTimeKind.Local));

        await using var serviceProvider = SetupDiscountServiceProvider(currentTime, CreateValentinesSpecialSettings);
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();
        var cart = CreateTestCart();
        cart = await discountService.ApplyDiscountAsync(cart);
        cart.Items[0].CartItemDiscounts.Should().HaveCount(1);
        cart.CartDiscounts.Should().HaveCount(0);
    }

    [Fact]
    public async Task TestAndDiscountAction() {
        var currentTime = AsUtc(new DateTime(2022, 12, 10, 0, 0, 0, DateTimeKind.Local));
        await using var serviceProvider = SetupDiscountServiceProvider(currentTime, CreateChristmasSpecial);
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();
        var cart = CreateTestCart();
        cart = await discountService.ApplyDiscountAsync(cart);
        cart.Items[0].CartItemDiscounts.Should().HaveCount(2);
        cart.CartDiscounts.Should().HaveCount(0);
    }
    
    [Fact]
    public async Task CartWithPriceAlreadyZero() {
        var currentTime = AsUtc(new DateTime(2022, 12, 10, 0, 0, 0, DateTimeKind.Local));
        await using var serviceProvider = SetupDiscountServiceProvider(currentTime, CreateFixedValueSetting);
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();
        var cart = CreateZeroCart();
        cart = await discountService.ApplyDiscountAsync(cart);
        cart.Items[0].CartItemDiscounts.Should().HaveCount(0);
        cart.CartDiscounts.Should().HaveCount(0);
    }

    private ServiceProvider SetupDiscountServiceProvider(DateTimeOffset currentTime,
        Func<IOptions<DiscountJsonOptions>, List<DiscountSettingDataModel>> discountSettings) {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IDiscountSettingRepository, DiscountSettingsRepository>();
        serviceCollection.AddCaasDiscountCore();
        serviceCollection.AddCaasDiscountInfrastructure();
        serviceCollection.AddSingleton<ITenantIdAccessor>(new StaticTenantIdAccessor(TestShopId.ToString()));

        serviceCollection.AddSingleton<IDateTimeOffsetProvider>(new StaticDateTimeOffsetProvider(currentTime));
        serviceCollection.AddSingleton<IDao<DiscountSettingDataModel>>(sp
            => new MemoryDao<DiscountSettingDataModel>(discountSettings.Invoke(sp.GetRequiredService<IOptions<DiscountJsonOptions>>())));
        serviceCollection.AddSingleton<IValidator, MockValidator>();

        return serviceCollection.BuildServiceProvider();
    }

    private List<DiscountSettingDataModel> CreateBlackFridaySettings(IOptions<DiscountJsonOptions> jsonOptions) {
        return new List<DiscountSettingDataModel>() {
            new DiscountSettingDataModel() {
                ShopId = TestShopId,
                Name = "Black Friday",
                RuleId = TimeWindowDiscountRule.Id,
                RuleParameters = new TimeWindowDiscountSettings() {
                    FromTime = AsUtc(new DateTime(2022, 11, 25, 0, 0, 0, DateTimeKind.Local)),
                    ToTime = AsUtc(new DateTime(2022, 11, 26, 0, 0, 0, DateTimeKind.Local))
                }.SerializeToElement(jsonOptions.Value.JsonSerializerOptions),
                ActionId = PercentageDiscountAction.Id,
                ActionParameters = new PercentageDiscountSettings() {
                    Percentage = 0.25m
                }.SerializeToElement(jsonOptions.Value.JsonSerializerOptions)
            }
        };
    }

    
    private List<DiscountSettingDataModel> CreateFixedValueSetting(IOptions<DiscountJsonOptions> jsonOptions) {
        return new List<DiscountSettingDataModel>() {
            new DiscountSettingDataModel() {
                ShopId = TestShopId,
                Name = "Some fixed value setting",
                RuleId = TimeWindowDiscountRule.Id,
                RuleParameters = new TimeWindowDiscountSettings() {
                    FromTime = AsUtc(new DateTime(2022, 01, 01, 0, 0, 0, DateTimeKind.Local)),
                    ToTime = AsUtc(new DateTime(2022, 12, 31, 0, 0, 0, DateTimeKind.Local))
                }.SerializeToElement(jsonOptions.Value.JsonSerializerOptions),
                ActionId = FixedValueDiscountAction.Id,
                ActionParameters = new FixedValueDiscountSettings() {
                    Value = 100
                }.SerializeToElement(jsonOptions.Value.JsonSerializerOptions)
            }
        };
    }
    
    private List<DiscountSettingDataModel> CreateValentinesSpecialSettings(IOptions<DiscountJsonOptions> jsonOptions) {
        return new List<DiscountSettingDataModel>() {
            new DiscountSettingDataModel() {
                ShopId = TestShopId,
                Name = "Valentines Special",
                RuleId = CompositeDiscountRule.Id,
                RuleParameters = new CompositeDiscountRuleSettings() {
                    DiscountSettings = new DiscountSettingMetadata[] {
                        new() {
                            Id = TimeWindowDiscountRule.Id,
                            Parameters = new TimeWindowDiscountSettings() {
                                FromTime = AsUtc(new DateTime(2023, 02, 14, 0, 0, 0, DateTimeKind.Local)),
                                ToTime = AsUtc(new DateTime(2023, 02, 15, 0, 0, 0, DateTimeKind.Local))
                            }
                        },
                        new() {
                            Id = MinProductCountDiscountRule.Id,
                            Parameters = new MinProductCountSettings() {
                                ProductId = UsbCableProductId,
                                NumberOfItems = 1
                            }
                        }
                    },
                    CombinationType = DiscountCombinationType.And
                }.SerializeToElement(jsonOptions.Value.JsonSerializerOptions),
                ActionId = PercentageDiscountAction.Id,
                ActionParameters = new PercentageDiscountSettings() {
                    Percentage = 0.10m
                }.SerializeToElement(jsonOptions.Value.JsonSerializerOptions)
            }
        };
    }
    
    private List<DiscountSettingDataModel> CreateChristmasSpecial(IOptions<DiscountJsonOptions> jsonOptions) {
        return new List<DiscountSettingDataModel>() {
            new DiscountSettingDataModel() {
                ShopId = TestShopId,
                Name = "Christmas Special",
                RuleId = CompositeDiscountRule.Id,
                RuleParameters = new CompositeDiscountRuleSettings() {
                    DiscountSettings = new DiscountSettingMetadata[] {
                        new() {
                            Id = TimeWindowDiscountRule.Id,
                            Parameters = new TimeWindowDiscountSettings() {
                                FromTime = AsUtc(new DateTime(2022, 12, 20, 0, 0, 0, DateTimeKind.Local)),
                                ToTime = AsUtc(new DateTime(2022, 12, 30, 0, 0, 0, DateTimeKind.Local))
                            }
                        },
                        new() {
                            Id = MinProductCountDiscountRule.Id,
                            Parameters = new MinProductCountSettings() {
                                ProductId = UsbCableProductId,
                                NumberOfItems = 1
                            }
                        }
                    },
                    CombinationType = DiscountCombinationType.Or
                }.SerializeToElement(jsonOptions.Value.JsonSerializerOptions),
                ActionId = AndDiscountAction.Id,
                ActionParameters = new AndDiscountActionSettings() {
                    DiscountSettings = new DiscountSettingMetadata[] {
                        new () {
                            Id = FixedValueDiscountAction.Id,
                            Parameters = new FixedValueDiscountSettings() {
                                Value = 2
                            }                        
                        },
                        new() {
                            Id = PercentageDiscountAction.Id,
                            Parameters = new PercentageDiscountSettings() {
                                Percentage = 0.05m
                            }
                        }
                    }
                }.SerializeToElement(jsonOptions.Value.JsonSerializerOptions)
            }
        };
    }

    private Cart CreateTestCart() {
        var productA = new Product() {
            Id = UsbCableProductId,
            Name = "USB cable",
            Price = 12.99m
        };

        var productB = new Product() {
            Id = HdmiCableProductId,
            Name = "HDMI cable",
            Price = 22.97m
        };

        var cart = new Cart() {
            ShopId = TestShopId,
            Items = new[] {
                new CartItem() { Product = productA, Amount = 3 },
                new CartItem() { Product = productB, Amount = 7 }
            }.ToImmutableArray()
        };
        return cart;
    }
    
    private Cart CreateZeroCart() {
        var productA = new Product() {
            Id = UsbCableProductId,
            Name = "USB cable",
            Price = 0m
        };
        
        var cart = new Cart() {
            ShopId = TestShopId,
            Items = new[] {
                new CartItem() { Product = productA, Amount = 1 },
            }.ToImmutableArray()
        };
        return cart;
    }

    private static DateTimeOffset AsUtc(DateTime dateTime) => new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime));
}