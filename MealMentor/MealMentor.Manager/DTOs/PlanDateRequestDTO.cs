namespace MealMentor.Manager.DTOs
{
    public class PlanDateRequestDTO
    {
        public string Description { get; set; }
        public DateTime PlanDate { get; set; }
        public List<PlanDateDetailRequestDTO> Details { get; set; }
    }
    public class PlanDateDetailRequestDTO
    {
        public List<string> Meals { get; set; }
        public string Type { get; set; }
        public TimeSpan PlanTime { get; set; }
    }
}
