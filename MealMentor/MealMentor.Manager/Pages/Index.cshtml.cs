using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using LoginDTO = MealMentor.Manager.DTOs.AccountDTO.LoginDTO;
using ResultModel = MealMentor.Manager.DTOs.ResultModel.ResultModel;

namespace MealMentor.Manager.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public LoginDTO LoginDTO { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(LoginDTO), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://meal-mentor.uydev.id.vn/api/Account/login", jsonContent);

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ResultModel>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result != null && result.IsSuccess)
                {
                    return RedirectToPage("/AdminPages/ManagerPage");
                }
                else
                {
                    Message = result?.Message ?? "Login failed";
                    ModelState.AddModelError(string.Empty, $"Login failed: {Message}");
                }
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
                ModelState.AddModelError(string.Empty, Message);
            }

            return Page();
        }
    }
}
