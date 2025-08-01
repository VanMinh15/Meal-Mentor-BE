using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MealMentor.Core.DTOs
{
    public class RecipeDTO
    {

        [Required]
        public string Name { get; set; }
        public string? TranslatedName { get; set; }

        public string? Description { get; set; }

        public string? Instruction { get; set; }

        public string? Preparation { get; set; }
        [JsonProperty("ingredients")]
        public List<string>? Ingredients { get; set; }
        public List<string>? TranslatedIngredients { get; set; }
        public string? Image { get; set; }
        public string? Url { get; set; }

    }

    public class RecipeUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? TranslatedName { get; set; }

        public string? Description { get; set; }

        public string? Instruction { get; set; }

        public string? Preparation { get; set; }
        [JsonProperty("ingredients")]
        public List<string>? Ingredients { get; set; }
        public List<string>? TranslatedIngredients { get; set; }

        public string? Image { get; set; }
        public string? Url { get; set; }

    }

}
