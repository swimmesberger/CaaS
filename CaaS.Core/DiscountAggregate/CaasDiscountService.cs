using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

public class CaasDiscountService : IDiscountService {
    private readonly IDiscountSettingRepository _discountSettingRepository;
    private readonly IDiscountComponentFactory _discountComponentFactory;

    public CaasDiscountService(IDiscountSettingRepository discountSettingRepository, IDiscountComponentFactory discountComponentFactory) {
        _discountSettingRepository = discountSettingRepository;
        _discountComponentFactory = discountComponentFactory;
    }

    public Task<IEnumerable<DiscountComponentMetadata>> GetDiscountActions() 
        => Task.FromResult(_discountComponentFactory.GetDiscountMetadata().Where(d => d.ComponentType == DiscountComponentType.Action));

    public Task<IEnumerable<DiscountComponentMetadata>> GetDiscountRules()
        => Task.FromResult(_discountComponentFactory.GetDiscountMetadata().Where(d => d.ComponentType == DiscountComponentType.Rule));

    public async Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default) {
        var discountComponents = (await _discountSettingRepository.FindAllAsync(cancellationToken))
            .Select(_discountComponentFactory.CreateComponent).ToList();
        return await RuleCheckDiscountAction.ForAll(discountComponents).ApplyDiscountAsync(cart, cancellationToken);
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
            var rule = new CompositeDiscountRule(discountComponents);
            var action = new AndDiscountAction(discountComponents);
            return new RuleCheckDiscountAction(new DiscountComponent(rule, action));
        }
    }
}