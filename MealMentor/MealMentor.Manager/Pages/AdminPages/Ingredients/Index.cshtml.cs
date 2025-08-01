using MealMentor.Manager.DTOs;
using MealMentor.Manager.DTOs.ResultModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Web;

namespace MealMentor.Manager.Pages.AdminPages.Ingredients
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<IngredientResponseDTO> Ingredients { get; set; } = new List<IngredientResponseDTO>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OnGetAsync(string? searchTerm, List<string?>? blackList, int currentPage = 1, int pageSize = 10)
        {
            await FetchIngredientsAsync(searchTerm, blackList, currentPage, pageSize);
        }

        public async Task<IActionResult> OnPostSearchAsync(string? searchTerm, List<string?>? blackList, int currentPage = 1, int pageSize = 10)
        {
            await FetchIngredientsAsync(searchTerm, blackList, currentPage, pageSize);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, string? description, string? name, string translatedName, string? image, int currentPage, int pageSize)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(description))
            {
                query["description"] = description;
            }
            if (!string.IsNullOrEmpty(name))
            {
                query["name"] = name;
            }
            if (!string.IsNullOrEmpty(translatedName))
            {
                query["translatedName"] = translatedName;
            }
            if (!string.IsNullOrEmpty(image))
            {
                query["image"] = image;
            }

            var requestUri = $"https://meal-mentor.uydev.id.vn/api/Ingredient/{id}?{query}";

            var response = await _httpClient.PutAsync(requestUri, null);

            if (response.IsSuccessStatusCode)
            {
                await FetchIngredientsAsync(null, null, currentPage, pageSize);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
            }
            return Page();
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id, int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;

            var response = await _httpClient.DeleteAsync($"https://meal-mentor.uydev.id.vn/api/Ingredient/{id}");

            if (response.IsSuccessStatusCode)
            {
                await FetchIngredientsAsync(null, null, CurrentPage, PageSize);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
            }
            return Page();
        }

        private async Task FetchIngredientsAsync(string? searchTerm, List<string?>? blackList, int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;

            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query["searchTerm"] = searchTerm;
            }
            query["PageNumber"] = currentPage.ToString();
            query["PageSize"] = pageSize.ToString();

            var requestUri = $"https://meal-mentor.uydev.id.vn/api/Ingredient?{query}";
            var requestBody = blackList ?? new List<string?>();

            var response = await _httpClient.PostAsJsonAsync(requestUri, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                try
                {
                    // Try to deserialize as PagedResult
                    var result = JsonConvert.DeserializeObject<ResultModel>(responseContent);

                    if (result != null && result.IsSuccess)
                    {
                        Ingredients = JsonConvert.DeserializeObject<List<IngredientResponseDTO>>(result.Data.ToString()) ?? new List<IngredientResponseDTO>();
                        TotalPages = (int)Math.Ceiling((double)Ingredients.Count / pageSize);
                    }
                    else
                    {
                        Ingredients = new List<IngredientResponseDTO>();
                        TotalPages = 1;
                    }
                }
                catch (JsonSerializationException)
                {
                    Ingredients = JsonConvert.DeserializeObject<List<IngredientResponseDTO>>(responseContent) ?? new List<IngredientResponseDTO>();
                    TotalPages = (int)Math.Ceiling((double)Ingredients.Count / pageSize);
                }
            }
            else
            {
                Ingredients = new List<IngredientResponseDTO>();
                TotalPages = 1;
            }
        }
    }
}
