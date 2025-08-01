namespace MealMentor.Core.DTOs
{
    public class AdminReportDTO
    {
        public object WeeklyRevenue { get; set; }
        public object LatestOrders { get; set; }
        public object RecipeCountEveryMonth { get; set; }
        public object MostLikedRecipe { get; set; }
        public object WeeklyUserCount { get; set; }
        public object WeeklyPaidUserCount { get; set; }
        public object TotalRecipeCount { get; set; }
        public object TotalUser { get; set; }
        public object TotalPaidUser { get; set; }
        public object TotalRevenue { get; set; }
    }
}
