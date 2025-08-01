using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Repositories.PaymentRepository
{
    public interface IPaymentRepository
    {
        Task<Order> AddOrder(Order order);
        Task<Order> UpdateOrder(Order order);
        Task<int> GetNewOrderCode();
        bool OrderExists(long orderCode);
        Task<Subscription> AddSubscription(Subscription subscription);
        Task<Order> GetOrderByOrderCode(long orderCode);
        Task<List<Order>> GetPaidOrdersInMonth(int year, int month);
        Task<PagedResult<OrderResponseDTO>> GetLatestOrders(PaginationParams paginationParams);
        Task<Subscription> AddSubcription(Subscription subscription);
    }
}
