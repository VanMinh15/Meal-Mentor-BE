namespace MealMentor.Core.DTOs.UserDTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Status { get; set; }
        public string? RecipeList { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public List<RecipeDTO> LikedRecipes { get; set; }
    }
}
