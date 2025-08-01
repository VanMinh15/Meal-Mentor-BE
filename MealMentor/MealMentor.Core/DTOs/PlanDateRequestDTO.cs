namespace MealMentor.Core.DTOs
{
    public class PlanDateRequestDTO
    {
        public string Description { get; set; }
        public DateTime PlanDate { get; set; }
        public List<PlanDateDetailRequestDTO> Details { get; set; }
    }
    public class PlanDateDetailRequestDTO
    {
        public List<int> Meals { get; set; }
        public string Type { get; set; }
    }
    public class PlanDateResponseDTO
    {
        public int Id { get; set; }
        public DateTime PlanDate { get; set; }
        public List<PlanDateDetailResponseDTO> Details { get; set; }
    }
    public class PlanDateDetailResponseDTO
    {
        public List<RecipeResponseDTO> Meal { get; set; }
        public string Type { get; set; }
    }
}
