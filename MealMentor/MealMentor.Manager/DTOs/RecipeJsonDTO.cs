using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MealMentor.Manager.DTOs
{
    public class RecipeJsonDTO
    {
        [Required]
        [JsonProperty("label")]
        public string Name { get; set; }

        [JsonProperty("ingredientLines")]
        public List<string> IngredientLines { get; set; }

        [JsonProperty("ingredients")]
        public List<ParsedIngredient> Ingredients { get; set; }

        [JsonProperty("totalNutrients")]
        public TotalNutrients TotalNutrients { get; set; }

        [JsonProperty("totalDaily")]
        public TotalNutrients TotalDaily { get; set; }

        [JsonProperty("totalWeight")]
        public double? TotalWeight { get; set; }

        [JsonProperty("image")]
        public string? Image { get; set; }

        [JsonProperty("uri")]
        public string? Url { get; set; }

        [JsonProperty("calories")]
        public double? Calories { get; set; }

    }

}
