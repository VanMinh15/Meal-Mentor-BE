using Newtonsoft.Json;

namespace MealMentor.Manager.DTOs
{
    public class RecipeListResult
    {
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("items")]
        public List<RecipeResponseDTO> Recipes { get; set; }
    }

}
