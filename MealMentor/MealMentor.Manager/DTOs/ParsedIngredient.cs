namespace MealMentor.Manager.DTOs
{
    public class ParsedIngredient
    {
        public double Quantity { get; set; }
        public string Measure { get; set; }
        public string? FoodMatch { get; set; }
        public string Food { get; set; }
        public string FoodId { get; set; }
        public double Weight { get; set; }
        public double? RetainedWeight { get; set; }
        public TotalNutrients Nutrients { get; set; }
        public string? MeasureURI { get; set; }
        public string Status { get; set; } = "Active";
        public string? Image { get; set; }
    }
}
