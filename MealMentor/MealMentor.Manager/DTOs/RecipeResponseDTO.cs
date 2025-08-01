using MealMentor.Core.Domain.Entities;

namespace MealMentor.Manager.DTOs
{
    public class RecipeResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TranslatedName { get; set; }

        public string? Description { get; set; }

        public string? Instruction { get; set; }

        public string? Preparation { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public string? Accessibility { get; set; }

        public double? Calories { get; set; }

        public double? TotalWeight { get; set; }

        public TotalNutrients TotalNutrients { get; set; }

        public TotalNutrients? TotalDaily { get; set; }

        public List<string> Ingredients { get; set; }
        public List<string> TranslatedIngredients { get; set; }

        public string? MealType { get; set; }

        public string? DishType { get; set; }

        public string? IsDeleted { get; set; }

        public string? LikeQuantity { get; set; }
        public string? Image { get; set; }
        public string? Url { get; set; }
        public virtual User? CreatedByNavigation { get; set; }
    }
}
