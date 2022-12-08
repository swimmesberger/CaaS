using System.Collections.Immutable;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

public class CaasDiscountService : IDiscountService {
    private readonly IDiscountSettingRepository _discountSettingRepository;
    private readonly IDiscountComponentFactory _discountComponentFactory;
    private readonly ITenantIdAccessor _tenantIdAccessor;

    public CaasDiscountService(IDiscountSettingRepository discountSettingRepository, IDiscountComponentFactory discountComponentFactory, ITenantIdAccessor tenantIdAccessor) {
        _discountSettingRepository = discountSettingRepository;
        _discountComponentFactory = discountComponentFactory;
        _tenantIdAccessor = tenantIdAccessor;
    }
    
    public IEnumerable<DiscountComponentMetadata> GetDiscountMetadata() => _discountComponentFactory.GetDiscountMetadata();

    public async Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default) {
        var discountComponents = (await _discountSettingRepository.FindAllAsync(cancellationToken))
            .Select(_discountComponentFactory.CreateComponent).ToList();
        return await RuleCheckDiscountAction.ForAll(discountComponents).ApplyDiscountAsync(cart, cancellationToken);
    }

    public async Task<DiscountSetting> AddDiscountSettingAsync(DiscountSetting discountSetting, CancellationToken cancellationToken = default) {
        discountSetting = discountSetting with {
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        
        return await _discountSettingRepository.AddAsync(discountSetting, cancellationToken);
    }

    public async Task<DiscountSetting> UpdateDiscountSettingAsync(Guid discountSettingId, DiscountSetting updatedDiscountSetting,
        CancellationToken cancellationToken = default) {
        var oldDiscountSetting = await _discountSettingRepository.FindByIdAsync(discountSettingId, cancellationToken);
        if (oldDiscountSetting == null) {
            throw new CaasItemNotFoundException($"discountSettingId '{oldDiscountSetting}' not found");
        }
        
        updatedDiscountSetting = updatedDiscountSetting with {
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };

        return await _discountSettingRepository.UpdateAsync(oldDiscountSetting, updatedDiscountSetting, cancellationToken);
    }
    
    public async Task DeleteDiscountSettingAsync(Guid discountSettingId, CancellationToken cancellationToken = default) {
        var discountSetting = await _discountSettingRepository.FindByIdAsync(discountSettingId, cancellationToken);
        if (discountSetting == null) {
            throw new CaasItemNotFoundException($"discountSettingId '{discountSetting}' not found");
        }
        
        await _discountSettingRepository.DeleteAsync(discountSetting, cancellationToken);
    }
    
    private class RuleCheckDiscountAction {
        private readonly DiscountComponent _discountComponent;

        public RuleCheckDiscountAction(DiscountComponent discountComponent) {
            _discountComponent = discountComponent;
        }

        public async Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default) {
            var ruleResult = await _discountComponent.Rule.Evaluate(cart, cancellationToken);
            if (ruleResult.Type == RuleResultType.None) 
                return cart;
            cart = await _discountComponent.Action.ApplyDiscountAsync(cart, ruleResult, cancellationToken);
            return cart;
        }

        public static RuleCheckDiscountAction ForAll(IReadOnlyCollection<DiscountComponent> discountComponents) {
            var rule = new CompositeDiscountRule(discountComponents.Select(c => c.Rule));
            var action = new AndDiscountAction(discountComponents.Select(c => c.Action));
            return new RuleCheckDiscountAction(new DiscountComponent(rule, action));
        }
    }
}