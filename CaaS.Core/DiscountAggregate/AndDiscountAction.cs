using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

public class AndDiscountAction : IDiscountAction {
    public static readonly Guid Id = new Guid("5B8D1040-23C6-45A6-A4EB-5B8495504A76");
    
    private readonly AndDiscountActionSettings _settings;
    private readonly IDiscountSettingRepository _discountSettingRepository;
    private readonly IDiscountComponentFactory _discountComponentFactory;
    
    private readonly IEnumerable<DiscountComponent>? _discountComponents;

    public AndDiscountAction(AndDiscountActionSettings settings, IDiscountSettingRepository discountSettingRepository, 
        IDiscountComponentFactory discountComponentFactory) {
        _settings = settings;
        _discountSettingRepository = discountSettingRepository;
        _discountComponentFactory = discountComponentFactory;
    }
    
    internal AndDiscountAction(IEnumerable<DiscountComponent> discountComponents) {
        _settings = null!;
        _discountSettingRepository = null!;
        _discountComponentFactory = null!;
        
        _discountComponents = discountComponents;
    }

    public async Task<Cart> ApplyDiscountAsync(Cart cart, RuleResult triggeredRule, CancellationToken cancellationToken = default) {
        foreach (var discountComponent in await GetDiscountComponents(cancellationToken)) {
            cart = await discountComponent.Action.ApplyDiscountAsync(cart, triggeredRule, cancellationToken);
        }
        return cart;
    }
    
    private async Task<IEnumerable<DiscountComponent>> GetDiscountComponents(CancellationToken cancellationToken = default) {
        IEnumerable<DiscountComponent> discountComponents;
        if (_discountComponents != null) {
            discountComponents = _discountComponents;
        } else {
            var discountSettings = await _discountSettingRepository.FindByIdsAsync(_settings.DiscountSettingIds, cancellationToken);
            return discountSettings.Select(_discountComponentFactory.CreateComponent);
        }
        return discountComponents;
    }
}