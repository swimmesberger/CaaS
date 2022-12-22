using AutoMapper;
using CaaS.Core.CustomerAggregate;

namespace CaaS.Api.CustomerApi.Models; 

public class CustomerProfile : Profile {
    public CustomerProfile() {
        CreateMap<Customer, CustomerDto>();
        CreateMap<CustomerForCreationDto, Customer>();
        CreateMap<CustomerForUpdateDto, Customer>();
    }
}