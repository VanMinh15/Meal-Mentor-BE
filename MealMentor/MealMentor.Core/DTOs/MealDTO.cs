using MealMentor.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MealMentor.Core.DTOs
{
    public class MealDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public RecipeAccessibility Accessibility { get; set; }
    }

    public class UpdateMealDTO
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public RecipeAccessibility Accessibility { get; set; }
    }
}
