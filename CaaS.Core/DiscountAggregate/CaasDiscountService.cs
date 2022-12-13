﻿using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.Base.Validation;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

public class CaasDiscountService : IDiscountService {
    private readonly IDiscountSettingRepository _discountSettingRepository;
    private readonly IDiscountComponentFactory _discountComponentFactory;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly IDiscountSettingRawConverter _settingRawConverter;
    private readonly IValidator _modelValidator;

    public CaasDiscountService(IDiscountSettingRepository discountSettingRepository, IDiscountComponentFactory discountComponentFactory, 
        ITenantIdAccessor tenantIdAccessor, IDiscountSettingRawConverter settingRawConverter, IValidator modelValidator) {
        _discountSettingRepository = discountSettingRepository;
        _discountComponentFactory = discountComponentFactory;
        _tenantIdAccessor = tenantIdAccessor;
        _settingRawConverter = settingRawConverter;
        _modelValidator = modelValidator;
    }
    
    public IEnumerable<DiscountComponentMetadata> GetDiscountMetadata() => _discountComponentFactory.GetDiscountMetadata();
    
    public async Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default) {
        var discountComponents = (await _discountSettingRepository.FindAllAsync(cancellationToken))
            .Select(_discountComponentFactory.CreateComponent).ToList();
        return await new CompositeDiscountApplier(discountComponents).ApplyDiscountAsync(cart, cancellationToken);
    }

    public async Task<DiscountSettingRaw> AddDiscountSettingAsync(DiscountSettingRaw discountSetting, CancellationToken cancellationToken = default) {
        discountSetting = discountSetting with {
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        var domainModel = DeserializeSetting(discountSetting);
        domainModel = await _discountSettingRepository.AddAsync(domainModel, cancellationToken);
        return _settingRawConverter.SerializeSetting(domainModel);
    }

    public async Task<DiscountSettingRaw> UpdateDiscountSettingAsync(DiscountSettingRaw updatedDiscountSetting, CancellationToken cancellationToken = default) {
        var oldDiscountSetting = await _discountSettingRepository.FindByIdAsync(updatedDiscountSetting.Id, cancellationToken);
        if (oldDiscountSetting == null) {
            throw new CaasItemNotFoundException($"discountSettingId '{oldDiscountSetting}' not found");
        }
        
        updatedDiscountSetting = updatedDiscountSetting with {
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        var domainModel = DeserializeSetting(updatedDiscountSetting);
        domainModel = await _discountSettingRepository.UpdateAsync(oldDiscountSetting, domainModel, cancellationToken);
        return _settingRawConverter.SerializeSetting(domainModel);
    }
    
    public async Task<IEnumerable<DiscountSettingRaw>> GetAllDiscountSettingsAsync(CancellationToken cancellationToken = default) {
        var result = await _discountSettingRepository.FindAllAsync(cancellationToken);
        return result.Select(discountSetting => _settingRawConverter.SerializeSetting(discountSetting)).ToList();
    }
    
    public async Task<DiscountSettingRaw> GetDiscountSettingByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var result = await _discountSettingRepository.FindByIdAsync(id, cancellationToken);
        if (result == null) {
            throw new CaasItemNotFoundException($"DiscountSetting {id} not found");
        }
        return _settingRawConverter.SerializeSetting(result);
    }

    public async Task DeleteDiscountSettingAsync(Guid discountSettingId, CancellationToken cancellationToken = default) {
        var discountSetting = await _discountSettingRepository.FindByIdAsync(discountSettingId, cancellationToken);
        if (discountSetting == null) {
            throw new CaasItemNotFoundException($"discountSettingId '{discountSetting}' not found");
        }
        
        await _discountSettingRepository.DeleteAsync(discountSetting, cancellationToken);
    }
    
    private DiscountSetting DeserializeSetting(DiscountSettingRaw rawSetting) {
        var discountSetting = _settingRawConverter.DeserializeSetting(rawSetting);
        if (!_modelValidator.TryValidateModel(discountSetting.Rule)){
            throw new CaasValidationException("rule not valid");
        }

        if (!_modelValidator.TryValidateModel(discountSetting.Action)) {
            throw new CaasValidationException("action not valid");
        }
        return discountSetting;
    }
}