using MealMentor.Core.DTOs.ResultModel;
using MealMentor.Manager.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using static MealMentor.Manager.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.Manager.Pages.AdminPages
{
    public class ReportModel : PageModel
    {
        public AdminReportDTO AdminReport { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }

        public async Task OnGet(int currentPage = 1)
        {
            CurrentPage = currentPage;
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"https://meal-mentor.uydev.id.vn/api/Admin/admin-report?PageNumber={CurrentPage}");
                var result = JsonConvert.DeserializeObject<ResultModel>(response);

                if (result.IsSuccess)
                {
                    AdminReport = JsonConvert.DeserializeObject<AdminReportDTO>(result.Data.ToString());
                    TotalPages = AdminReport.LatestOrders.TotalPages;
                }
            }
        }
    }

    public class AdminReportDTO
    {
        public Dictionary<int, int> WeeklyRevenue { get; set; }
        public PagedResult<OrderResponseDTO> LatestOrders { get; set; }
        public Dictionary<string, int> RecipeCountEveryMonth { get; set; }
        public List<MostLikedRecipeDTO> MostLikedRecipe { get; set; }
        public int WeeklyUserCount { get; set; }
        public WeeklyPaidUserCountDTO WeeklyPaidUserCount { get; set; }
        public int TotalRecipeCount { get; set; }
        public int TotalUser { get; set; }
        public int TotalPaidUser { get; set; }
        public double TotalRevenue { get; set; }
    }

    public class MostLikedRecipeDTO
    {
        public string RecipeName { get; set; }
        public int LikeQuantity { get; set; }
    }

    public class WeeklyPaidUserCountDTO
    {
        public int CurrentWeekCount { get; set; }
        public int PreviousWeekCount { get; set; }
    }
}
