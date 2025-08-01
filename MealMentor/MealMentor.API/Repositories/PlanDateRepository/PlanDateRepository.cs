using MealMentor.API.Database;
using MealMentor.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealMentor.API.Repositories.PlanDateRepository
{
    public class PlanDateRepository : IPlanDateRepository
    {
        private readonly MealMentorDbContext _context = MealMentorDbContext.Instance;

        public async Task<PlanDate> CreatePlanDate(PlanDate planDate)
        {
            var tar = await _context.PlanDates.FirstOrDefaultAsync(x => x.CreatedBy == planDate.CreatedBy && x.CreateDateTime == planDate.CreateDateTime);
            if (DateTime.Now.Date.AddDays(14) <= planDate.CreateDateTime) throw new Exception("400 - Cannot plan for more thans 2 weeks ahead!");
            if (tar == null)
            {
                await _context.PlanDates.AddAsync(planDate);
                await _context.SaveChangesAsync();
                return planDate;
            }
            return await UpdatePlanDate(planDate);
        }

        public async void DeletePlanDate(string userId, DateTime date)
        {
            var tar = await _context.PlanDates.FirstOrDefaultAsync(x => x.CreatedBy == userId && x.CreateDateTime == date.Date);
            if (tar == null)
            {
                _context.PlanDates.Remove(tar);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<PlanDate>> GetPlanDatesByUserId(string createdById)
        {
            return await _context.PlanDates.Include(x => x.PlanDateDetails).Where(x => x.CreateDateTime.Value.Date >= DateTime.Now.Date.AddDays(-14) && x.CreateDateTime.Value.Date <= DateTime.Now.Date.AddDays(14)).ToListAsync();
        }
        public async Task<PlanDate> GetPlanDateDetail(string userId, DateTime date)
        {
            return await _context.PlanDates.Include(x => x.PlanDateDetails).FirstOrDefaultAsync(x => x.CreatedBy == userId && x.CreateDateTime.Value.Date == date.Date);
        }

        public async Task<PlanDate> UpdatePlanDate(PlanDate planDate)
        {
            var tar = await _context.PlanDates.FirstOrDefaultAsync(x => x.CreatedBy == planDate.CreatedBy && x.CreateDateTime == planDate.CreateDateTime);
            if (tar == null)
            {
                return await CreatePlanDate(planDate);
            }
            tar.PlanDateDetails = planDate.PlanDateDetails;
            tar.CreateDateTime = planDate.CreateDateTime;
            tar.Accessibility = planDate.Accessibility;
            await _context.SaveChangesAsync();
            return tar;
        }

    }
}
