using AutoMapper;
using CaaS.Core.OrderAggregate;

namespace CaaS.Api.OrderApi.Models; 

public class OrderProfile : Profile {
    public OrderProfile() {
        CreateMap<Order, OrderDto>().ForMember(o => o.CustomerId,
            opt => opt.MapFrom(src => src.Customer.Id));
        
        CreateMap<OrderItem, OrderItemDto>();
    }
}