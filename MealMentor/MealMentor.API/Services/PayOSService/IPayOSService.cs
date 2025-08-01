using MealMentor.Core.DTOs.ResultModel;
using Net.payOS.Types;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.PayOSService
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePaymentLink(string userId);
        Task<ResultModel> ProcessPayment(long orderCode);
        Task<ResultModel> GetWeeklyRevenue();
        Task<ResultModel> GetLatestOrders(PaginationParams paginationParams);
    }
}
