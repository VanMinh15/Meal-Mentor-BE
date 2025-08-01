using MealMentor.API.Database;
using MealMentor.API.Pagination;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.Domain.Enums;
using MealMentor.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Repositories.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly MealMentorDbContext _context = MealMentorDbContext.Instance;

        public async Task<Order> AddOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            var tar = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == order.OrderCode);
            tar.Status = order.Status;
            await _context.SaveChangesAsync();
            return tar;
        }

        public async Task<int> GetNewOrderCode()
        {
            int orderCode;
            bool exists;
            object _lock = new object();
            Random _random = new Random();
            do
            {
                lock (_lock)
                {
                    orderCode = _random.Next(1, 20001);
                }

                exists = await _context.Orders.AnyAsync(o => o.OrderCode == orderCode);
            } while (exists);

            return orderCode;
        }

        public bool OrderExists(long orderCode)
        {
            return _context.Orders.Any(o => o.OrderCode == orderCode);
        }

        public async Task<Subscription> AddSubscription(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Order> GetOrderByOrderCode(long orderCode)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderCode.Value == int.Parse(orderCode.ToString()));
            return order;
        }

        public async Task<List<Order>> GetPaidOrdersInMonth(int year, int month)
        {
            return await _context.Orders
                .Where(o => o.Status == PaymentEnums.Paid.ToString() && o.CreatedDateAt.HasValue && o.CreatedDateAt.Value.Year == year && o.CreatedDateAt.Value.Month == month)
                .ToListAsync();
        }

        public async Task<PagedResult<OrderResponseDTO>> GetLatestOrders(PaginationParams paginationParams)
        {
            var query = _context.Orders
                .Where(o => o.Status == PaymentEnums.Paid.ToString())
                .OrderByDescending(o => o.CreatedDateAt)
                .AsQueryable();

            var pagedOrders = await query.ToPagedResultAsync(paginationParams);

            var orderResponseDTOs = new List<OrderResponseDTO>();

            foreach (var order in pagedOrders.Items)
            {
                var user = await _context.Users.FindAsync(order.UserId);
                var subscription = await _context.Subscriptions
                    .Where(s => s.UserId == order.UserId)
                    .OrderByDescending(s => s.CreatedDate)
                    .FirstOrDefaultAsync();

                orderResponseDTOs.Add(new OrderResponseDTO
                {
                    UserName = user?.Username,
                    PlanId = subscription?.PlanId ?? 0,
                    LifeTime = ((subscription.EndDate.Value.Year - subscription.StartDate.Value.Year) * 12) + subscription.EndDate.Value.Month - subscription.StartDate.Value.Month,

                    Price = order.Price ?? 0
                });
            }

            return new PagedResult<OrderResponseDTO>
            {
                Items = orderResponseDTOs,
                TotalCount = pagedOrders.TotalCount,
                PageSize = pagedOrders.PageSize,
                CurrentPage = pagedOrders.CurrentPage
            };
        }

        public async Task<Subscription> AddSubcription(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }
    }
}
