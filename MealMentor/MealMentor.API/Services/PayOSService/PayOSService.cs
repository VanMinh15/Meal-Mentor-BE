using MealMentor.API.Repositories.PaymentRepository;
using MealMentor.API.Repositories.UserRepository;
using MealMentor.API.Services.UserService;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.Domain.Enums;
using MealMentor.Core.DTOs.ResultModel;
using Net.payOS;
using Net.payOS.Types;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.PayOSService
{
    public class PayOSService : IPayOSService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IConfiguration _configuration;
        private readonly PayOS _payOS;

        public PayOSService(IUserService userService, IUserRepository userRepository, PayOS payOS, IPaymentRepository paymentRepository, IConfiguration configuration)
        {
            _paymentRepository = paymentRepository;
            _userService = userService;
            _userRepository = userRepository;
            _payOS = payOS;
        }

        public async Task<CreatePaymentResult> CreatePaymentLink(string userId)
        {
            try
            {
                int orderCode = await _paymentRepository.GetNewOrderCode();
                var order = new Order
                {
                    UserId = userId,
                    OrderCode = orderCode,
                    Price = 50000,
                    Status = PaymentEnums.Pending.ToString(),
                    CreatedDateAt = DateTime.Now,
                };

                await _paymentRepository.AddOrder(order);

                int expiredAt = (int)(DateTime.UtcNow.AddMinutes(10) - new DateTime(1970, 1, 1)).TotalSeconds;

                PaymentData paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: 50000,
                    description: $"Subscription Meal Mentor",
                    items: new List<ItemData>(),
                    cancelUrl: "https://meal-mentor.uydev.id.vn/swagger/payment-failed/",
                    returnUrl: "https://meal-mentor.uydev.id.vn/swagger/payment-success/",
                    expiredAt: expiredAt
                );

                CreatePaymentResult createPaymentResult = await _payOS.createPaymentLink(paymentData);
                return createPaymentResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResultModel> ProcessPayment(long orderCode)
        {
            try
            {
                var orderExists = await _paymentRepository.GetOrderByOrderCode(orderCode);
                if (orderExists == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Order not found",
                        Data = new PaymentResult
                        {
                            Success = false,
                            Message = "Order not found",
                            StatusCode = 404
                        }
                    };
                }

                orderExists.Status = PaymentEnums.Paid.ToString();
                await _paymentRepository.UpdateOrder(orderExists);

                var user = await _userRepository.GetUserById(orderExists.UserId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "User not found",
                        Data = new PaymentResult
                        {
                            Success = false,
                            Message = "User not found",
                            StatusCode = 404
                        }
                    };
                }

                var subscription = new Subscription
                {
                    UserId = user.Id,
                    PlanId = 1,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    Status = SubscriptionStatus.Active.ToString(),
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now
                };
                await _paymentRepository.AddSubscription(subscription);

                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Payment processed successfully",
                    Data = new PaymentResult
                    {
                        Success = true,
                        Message = "Payment processed successfully",
                        StatusCode = 200
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = new PaymentResult
                    {
                        Success = false,
                        Message = ex.Message,
                        StatusCode = 500
                    }
                };
            }
        }
        public async Task<ResultModel> GetWeeklyRevenue()
        {
            var result = new ResultModel();
            try
            {
                var currentDate = DateTime.Now;
                var currentMonth = currentDate.Month;
                var currentYear = currentDate.Year;

                var orders = await _paymentRepository.GetPaidOrdersInMonth(currentYear, currentMonth);

                // Define the weeks we are interested in
                var targetWeeks = new[] { 1, 2, 3, 4, 5 };

                // Initialize the dictionary with target weeks and set their values to 0
                var weeklyRevenue = targetWeeks.ToDictionary(week => week, week => 0m);

                // Group orders by week within the month and filter for the target weeks
                var groupedOrders = orders
                    .GroupBy(o => GetWeekOfMonth(o.CreatedDateAt.Value))
                    .Where(g => targetWeeks.Contains(g.Key));

                // Update the dictionary with actual revenue data
                foreach (var group in groupedOrders)
                {
                    weeklyRevenue[group.Key] = (decimal)group.Sum(o => o.Price ?? 0);
                }

                result.IsSuccess = true;
                result.Data = weeklyRevenue;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private int GetWeekOfMonth(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var weekNumber = (date.Day + (int)firstDayOfMonth.DayOfWeek - 1) / 7 + 1;
            return weekNumber > 4 ? 5 : weekNumber;
        }


        public async Task<ResultModel> GetLatestOrders(PaginationParams paginationParams)
        {
            try
            {
                var pagedOrders = await _paymentRepository.GetLatestOrders(paginationParams);

                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Latest orders retrieved successfully",
                    Data = pagedOrders
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


    }
}
