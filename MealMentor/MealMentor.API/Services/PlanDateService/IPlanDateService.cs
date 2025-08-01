using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs.ResultModel;

namespace MealMentor.API.Services.PlanDateService
{
    public interface IPlanDateService
    {
        Task<ResultModel> CreatePlanDate(PlanDate planDate);
        Task<ResultModel> UpdatePlanDate(PlanDate planDate);
        Task<ResultModel> DeletePlanDate(DateTime planDate);
        Task<ResultModel> GetPlanDateByUserId();
        Task<ResultModel> GetWeekIngredient();

    }
}
