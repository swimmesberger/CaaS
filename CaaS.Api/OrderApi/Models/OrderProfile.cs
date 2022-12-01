using AutoMapper;
using CaaS.Core.OrderAggregate;

namespace CaaS.Api.OrderApi.Models; 

public class OrderProfile : Profile {
    public OrderProfile() {
        CreateMap<Order, OrderDto>().ForMember(o => o.BillingAddress,
                opt => opt.MapFrom(src => src.Address));

        CreateMap<OrderItem, OrderItemDto>();
    }
}