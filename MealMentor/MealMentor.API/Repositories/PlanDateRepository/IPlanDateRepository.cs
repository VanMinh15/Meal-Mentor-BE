using MealMentor.Core.Domain.Entities;

namespace MealMentor.API.Repositories.PlanDateRepository
{
    public interface IPlanDateRepository
    {
        Task<PlanDate> CreatePlanDate(PlanDate planDate);
        Task<PlanDate> UpdatePlanDate(PlanDate planDate);
        Task<List<PlanDate>> GetPlanDatesByUserId(string createdById);
        void DeletePlanDate(string userId, DateTime date);
        Task<PlanDate> GetPlanDateDetail(string userId, DateTime date);
    }
}
