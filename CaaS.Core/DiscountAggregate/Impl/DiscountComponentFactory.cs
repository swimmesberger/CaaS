using System.Reflection;
using CaaS.Core.Base.Di;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CaaS.Core.DiscountAggregate.Impl; 

public class DiscountComponentFactory : IDiscountComponentFactory {
    private readonly IReadOnlyDictionary<Guid, DiscountComponentMetadata> _discountComponents;
    private readonly IServiceProvider _serviceProvider;
    private readonly MethodInfo _createOptionsMethod;
    
    public DiscountComponentFactory(IEnumerable<DiscountComponentMetadata> componentMetadata, 
        IServiceProvider serviceProvider) {
        _discountComponents = componentMetadata.ToDictionary(c => c.Id);
        _serviceProvider = serviceProvider;
        _createOptionsMethod = GetType()
            .GetMethod(nameof(CreateOptions), BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException();
    }

    public IEnumerable<DiscountComponentMetadata> GetDiscountMetadata() => _discountComponents.Values;
    
    public DiscountComponent CreateComponent(DiscountSetting discountSetting) {
        if (!_discountComponents.TryGetValue(discountSetting.Rule.Id, out var rule)) {
            throw new CaasDiscountException($"Can't find discount rule with id {discountSetting.Id}");
        }
        if (!_discountComponents.TryGetValue(discountSetting.Action.Id, out var action)) {
            throw new CaasDiscountException($"Can't find discount action with id {discountSetting.Id}");
        }
        return new DiscountComponent(
            Rule: CreateRule(rule, discountSetting.Rule.Parameters), 
            Action: CreateAction(action, discountSetting.Action.Parameters)
        );
    }

    public IDiscountRule CreateRule(DiscountSettingMetadata settingMetadata) {
        if (!_discountComponents.TryGetValue(settingMetadata.Id, out var rule)) {
            throw new CaasDiscountException($"Can't find discount rule with id {settingMetadata.Id}");
        }
        return CreateRule(rule, settingMetadata.Parameters);
    }

    public IDiscountAction CreateAction(DiscountSettingMetadata settingMetadata) {
        if (!_discountComponents.TryGetValue(settingMetadata.Id, out var action)) {
            throw new CaasDiscountException($"Can't find discount action with id {settingMetadata.Id}");
        }
        return CreateAction(action, settingMetadata.Parameters);
    }

    private IDiscountRule CreateRule(DiscountComponentMetadata component, DiscountParameters settings) {
        if (component.ComponentType != DiscountComponentType.Rule) throw new ArgumentException();
        return (IDiscountRule)ActivatorUtilities.CreateInstance(CreateDiscountOptionsProvider(settings), component.ServiceType);
    }
    
    private IDiscountAction CreateAction(DiscountComponentMetadata component, DiscountParameters settings) {
        if (component.ComponentType != DiscountComponentType.Action) throw new ArgumentException();
        return (IDiscountAction)ActivatorUtilities.CreateInstance(CreateDiscountOptionsProvider(settings), component.ServiceType);
    }

    private IServiceProvider CreateDiscountOptionsProvider(DiscountParameters settings) {
        object? GetService(Type serviceType) {
            if (!serviceType.IsGenericType || serviceType.GetGenericTypeDefinition() != typeof(IDiscountOptions<>)) 
                return _serviceProvider.GetService(serviceType);
            
            var genericType = serviceType.GetGenericArguments()[0];
            var creatOpMethod = _createOptionsMethod.MakeGenericMethod(genericType);
            var options = creatOpMethod.Invoke(this, new object?[]{ settings });
            return options;
        }
        return new FuncServiceProvider(GetService);
    }

    private IDiscountOptions<T> CreateOptions<T>(DiscountParameters settings) where T : DiscountParameters {
        if (settings is T cSettings) {
            return new DiscountOptions<T>(cSettings);
        }
        throw new ArgumentException($"Settings {settings.GetType()} is not of type {typeof(T)}");
    }
}