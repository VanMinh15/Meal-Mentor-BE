using MealMentor.API.Services.IngredientService;
using Microsoft.AspNetCore.Mvc;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }


        [HttpPost]
        public async Task<IActionResult> GetIngredients([FromQuery] string? searchTerm, List<string>? blackList, [FromQuery] PaginationParams pagingParams)
        {
            var result = await _ingredientService.GetIngredients(searchTerm, new(), blackList, pagingParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, string? description, string? name, string translatedName, string? image)
        {
            if (description == null && name == null && image == null) return StatusCode(400, "At least 1 field is required");
            var result = await _ingredientService.UpdateIngredient(id, description, name, translatedName, image);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _ingredientService.DeleteIngredient(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
