using MealMentor.API.Services.PlanDateService;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MealMentor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanDateController : ControllerBase
    {
        private readonly IPlanDateService _planDateService;
        public PlanDateController(IPlanDateService planDateService)
        {
            _planDateService = planDateService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlandate(PlanDateRequestDTO plandate)
        {
            var details = plandate.Details.Select(x => new PlanDateDetail()
            {
                Meal = JsonConvert.SerializeObject(x.Meals),
                PlanTime = DateTime.Now,
                Type = x.Type,

            }).ToList();
            PlanDate plan = new()
            {
                Accessibility = "Public",
                CreateDateTime = plandate.PlanDate,
                PlanDateDetails = details
            };
            var result = await _planDateService.CreatePlanDate(plan);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet()]
        public async Task<IActionResult> GetPlanDate()
        {
            var result = await _planDateService.GetPlanDateByUserId();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("WeekIngredients")]
        public async Task<IActionResult> GetWeekIngr()
        {
            var result = await _planDateService.GetWeekIngredient();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateRecipe(PlanDateRequestDTO plandate)
        {
            var details = plandate.Details.Select(x => new PlanDateDetail()
            {
                Meal = JsonConvert.SerializeObject(x.Meals),
                PlanTime = DateTime.Now,
                Type = x.Type
            }).ToList();
            PlanDate plan = new()
            {
                Accessibility = "Public",
                CreateDateTime = plandate.PlanDate,
                PlanDateDetails = details
            };
            var result = await _planDateService.UpdatePlanDate(plan);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete()]
        public async Task<IActionResult> DeleteRecipe(DateTime planDate)
        {
            var result = await _planDateService.DeletePlanDate(planDate.Date);
            return StatusCode(result.StatusCode, result);
        }

    }
}
