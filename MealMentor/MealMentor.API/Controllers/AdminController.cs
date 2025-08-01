using MealMentor.API.Services.PayOSService;
using MealMentor.API.Services.RecipeService;
using MealMentor.API.Services.UserService;
using MealMentor.Core.DTOs;
using MealMentor.Core.DTOs.ResultModel;
using Microsoft.AspNetCore.Mvc;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPayOSService _payOSService;
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;

        public AdminController(IPayOSService payOSService, IUserService userService, IRecipeService recipeService)
        {
            _payOSService = payOSService;
            _userService = userService;
            _recipeService = recipeService;
        }

        [HttpGet("admin-report")]
        public async Task<IActionResult> GetAdminReport([FromQuery] PaginationParams paginationParams)
        {
            var weeklyRevenueTask = await _payOSService.GetWeeklyRevenue();
            var latestOrdersTask = await _payOSService.GetLatestOrders(paginationParams);
            var recipeCountEveryMonthTask = await _recipeService.GetRecipeCountEveryMonth();
            var mostLikedRecipeTask = await _recipeService.GetMostLikedRecipe();
            var weeklyUserCountTask = await _userService.GetUserCountInWeek();
            var weeklyPaidUserCountTask = await _userService.GetWeeklyPaidUserCount();
            var totalRecipeCountTask = await _recipeService.GetRecipeTotalCount();
            var totalUser = await _userService.GetTotalUser();
            var totalPaidUser = await _userService.GetTotalPaidUser();
            var totalRevenue = await _userService.GetTotalRevenue();

            var adminReport = new AdminReportDTO
            {
                WeeklyRevenue = weeklyRevenueTask.Data,
                LatestOrders = latestOrdersTask.Data,
                RecipeCountEveryMonth = recipeCountEveryMonthTask.Data,
                MostLikedRecipe = mostLikedRecipeTask.Data,
                WeeklyUserCount = weeklyUserCountTask.Data,
                WeeklyPaidUserCount = weeklyPaidUserCountTask.Data,
                TotalRecipeCount = totalRecipeCountTask.Data,
                TotalUser = totalUser.Data,
                TotalPaidUser = totalPaidUser.Data,
                TotalRevenue = totalRevenue.Data
            };

            if (weeklyRevenueTask.IsSuccess && latestOrdersTask.IsSuccess && recipeCountEveryMonthTask.IsSuccess &&
                mostLikedRecipeTask.IsSuccess && weeklyUserCountTask.IsSuccess && weeklyPaidUserCountTask.IsSuccess
                && totalRecipeCountTask.IsSuccess && totalUser.IsSuccess && totalPaidUser.IsSuccess && totalRevenue.IsSuccess)
            {
                return Ok(new ResultModel
                {
                    IsSuccess = true,
                    Message = "Successfully retrieved admin report",
                    Data = adminReport,
                    StatusCode = 200
                });
            }

            return StatusCode(500, new ResultModel
            {
                IsSuccess = false,
                Message = "Failed to retrieve admin report",
                Data = null,
                StatusCode = 500
            });
        }
    }
}
