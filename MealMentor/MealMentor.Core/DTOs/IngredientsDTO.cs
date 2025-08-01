namespace MealMentor.Core.DTOs
{
    public class IngredientsDTO
    {
        public string Text { get; set; }
        public List<ParsedIngredient> Parsed { get; set; }
    }

    public class IngredientGetDTO
    {
        public List<string> vn { get; set; }
        public List<string> en { get; set; }
    }
}
