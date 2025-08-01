using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using IngredientsDTO = MealMentor.Manager.DTOs.IngredientsDTO;
using RecipeDTO = MealMentor.Manager.DTOs.RecipeDTO;
using RecipeListResult = MealMentor.Manager.DTOs.RecipeListResult;
using RecipeResponseDTO = MealMentor.Manager.DTOs.RecipeResponseDTO;
using RecipeUpdateDTO = MealMentor.Manager.DTOs.RecipeUpdateDTO;
using ResultModel = MealMentor.Manager.DTOs.ResultModel.ResultModel;

namespace MealMentor.Manager.Pages.AdminPages.Recipes
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            RecipeDTO = new RecipeDTO();
            Ingredients = new List<IngredientsDTO>();
            Message = string.Empty;
            Recipes = new List<RecipeResponseDTO>();
        }

        [BindProperty]
        public RecipeDTO RecipeDTO { get; set; }

        [BindProperty]
        public List<IngredientsDTO> Ingredients { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string Message { get; set; }
        public string Keyword { get; set; }
        public List<RecipeResponseDTO> Recipes { get; set; }



        public async Task<IActionResult> OnGetAsync(int currentPage)
        {
            if (currentPage < 1)
            {
                currentPage = 1;
            }

            CurrentPage = currentPage;
            var response = await _httpClient.GetAsync($"https://meal-mentor.uydev.id.vn/api/Recipe/get-all?pageNumber={CurrentPage}&pageSize={PageSize}");
            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var result = JsonConvert.DeserializeObject<ResultModel>(responseContent, settings);

                if (result != null && result.IsSuccess && result.Data != null)
                {
                    var dataString = result.Data.ToString().Trim(new char[] { '\uFEFF', '\u200B' });
                    if (!string.IsNullOrEmpty(dataString))
                    {
                        var recipeListResult = JsonConvert.DeserializeObject<RecipeListResult>(dataString, settings);
                        if (recipeListResult != null)
                        {
                            Recipes = recipeListResult.Recipes;
                            CurrentPage = recipeListResult.CurrentPage;
                            TotalPages = recipeListResult.TotalPages;
                        }
                        else
                        {
                            Message = "Failed to parse recipe list";
                            Recipes = new List<RecipeResponseDTO>();
                        }
                    }
                    else
                    {
                        Message = "Data is empty";
                        Recipes = new List<RecipeResponseDTO>();
                    }
                }
                else
                {
                    Message = result?.Message ?? "Failed to fetch recipes";
                }
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                Message = "Error deserializing JSON response.";
                Recipes = new List<RecipeResponseDTO>();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCreateRecipeAsync()
        {
            if (Ingredients == null || !Ingredients.Any())
            {
                ModelState.AddModelError(string.Empty, "Please enter at least one ingredient");
                return Page();
            }

            RecipeDTO.Ingredients = Ingredients.Select(i => i.Text).ToList();

            var jsonContent = new StringContent(JsonConvert.SerializeObject(RecipeDTO), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://meal-mentor.uydev.id.vn/api/Recipe", jsonContent);

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultModel>(responseContent);

            if (result != null && result.IsSuccess)
            {
                return RedirectToPage();
            }
            else
            {
                Message = result?.Message ?? "Failed to create recipe";
                ModelState.AddModelError(string.Empty, Message);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteRecipeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://meal-mentor.uydev.id.vn/api/Recipe/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResultModel>(responseContent);
                Message = result?.Message ?? "Failed to delete recipe";
                ModelState.AddModelError(string.Empty, Message);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSearchRecipeAsync(string keyword, int currentPage)
        {
            if (currentPage < 1)
            {
                currentPage = 1;
            }

            Keyword = keyword;
            CurrentPage = currentPage;

            var response = await _httpClient.GetAsync($"https://meal-mentor.uydev.id.vn/api/Recipe/search?keyword={keyword}&pageNumber={CurrentPage}&pageSize={PageSize}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResultModel>(responseContent);

                if (result != null && result.IsSuccess && result.Data != null)
                {
                    var dataString = result.Data.ToString().Trim(new char[] { '\uFEFF', '\u200B' });
                    if (!string.IsNullOrEmpty(dataString))
                    {
                        var recipeListResult = JsonConvert.DeserializeObject<RecipeListResult>(dataString);
                        if (recipeListResult != null)
                        {
                            Recipes = recipeListResult.Recipes;
                            CurrentPage = recipeListResult.CurrentPage;
                            TotalPages = recipeListResult.TotalPages;
                        }
                        else
                        {
                            Message = "Failed to parse recipe list";
                            Recipes = new List<RecipeResponseDTO>();
                        }
                    }
                    else
                    {
                        Message = "Data is empty";
                        Recipes = new List<RecipeResponseDTO>();
                    }
                }
                else
                {
                    Message = result?.Message ?? "Failed to fetch recipes";
                }
            }
            else
            {
                Message = "Error fetching data from API";
            }

            return await OnGetAsync(CurrentPage);
        }
        public async Task<IActionResult> OnPostUpdateRecipeAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!string.IsNullOrEmpty(RecipeDTO.TranslatedIngredientsString))
            {
                RecipeDTO.TranslatedIngredients = RecipeDTO.TranslatedIngredientsString
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(ti => ti.Trim())
                    .ToList();
            }
            if (RecipeDTO.Ingredients == null)
            {
                RecipeDTO.Ingredients = new List<string>();
            }
            var recipeUpdateDTO = new RecipeUpdateDTO
            {
                Id = id,
                Name = RecipeDTO.Name,
                TranslatedName = RecipeDTO.TranslatedName,
                Description = RecipeDTO.Description,
                Instruction = RecipeDTO.Instruction,
                Preparation = RecipeDTO.Preparation,
                Ingredients = RecipeDTO.Ingredients,
                TranslatedIngredients = RecipeDTO.TranslatedIngredients,
                Image = RecipeDTO.Image,
                Url = RecipeDTO.Url
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(recipeUpdateDTO), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"https://meal-mentor.uydev.id.vn/api/Recipe/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                Message = "Recipe updated successfully.";
            }
            else
            {
                Message = "Error updating recipe.";
            }

            return RedirectToPage();
        }
    }
}
