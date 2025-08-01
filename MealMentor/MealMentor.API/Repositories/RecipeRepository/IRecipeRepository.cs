using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs;

namespace MealMentor.API.Repositories.RecipeRepository
{
    public interface IRecipeRepository
    {
        Task<Recipe> GetRecipeById(int id);
        Task<Recipe> AddRecipe(Recipe recipe);
        IQueryable<Recipe> GetRecipeByName(string keyword);
        Task<Recipe> DeleteRecipe(int id);
        Task<Recipe> UpdateRecipe(int id, Recipe recipe);
        Task<List<Recipe>> GetAllRecipes();
        IQueryable<Recipe> GetRecipeList();
        Task<Recipe> LikeRecipe(int id);
        Task<Recipe> DislikeRecipe(int id);
        Task<Dictionary<string, int>> GetRecipeCountByMonth();
        Task<List<LikedRecipeDTO>> GetMostLikeRecipes();
        Task<int> GetRecipeTotalCount();
    }
}
