using MealMentor.API.Services.RecipeService;
using MealMentor.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService recipeService;
        public RecipeController(IRecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe(RecipeDTO recipeDTO)
        {
            var result = await recipeService.CreateRecipe(recipeDTO);
            return StatusCode(result.StatusCode, result);
        }
        //675592435664a1da4a4f51bcd9477017 Key
        //9d914e48 ID
        [HttpGet("search")]
        public async Task<IActionResult> SearchRecipe(string keyword, [FromQuery] PaginationParams pagingParams)
        {
            var result = await recipeService.SearchRecipe(keyword, pagingParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe([FromRoute] int id, [FromBody] RecipeUpdateDTO recipe)
        {
            var result = await recipeService.UpdateRecipe(id, recipe);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe([FromRoute] int id)
        {
            try
            {
                var result = await recipeService.DeleteRecipe(id);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetRecipe(string? keyword, [FromQuery] PaginationParams pagingParams)
        {
            var result = await recipeService.GetRecipesByName(keyword ?? "", pagingParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllRecipes([FromQuery] PaginationParams paginationParams, string? orderBy)
        {
            var result = await recipeService.GetAllRecipes(paginationParams, orderBy);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeById([FromRoute] int id)
        {
            var result = await recipeService.GetRecipeById(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}
