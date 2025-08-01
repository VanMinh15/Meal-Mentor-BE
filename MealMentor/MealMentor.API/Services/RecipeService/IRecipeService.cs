using MealMentor.Core.DTOs;
using MealMentor.Core.DTOs.ResultModel;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.RecipeService
{
    public interface IRecipeService
    {
        Task<ResultModel> CreateRecipe(RecipeDTO recipeDTO);
        Task<ResultModel> SearchRecipe(string keyword, PaginationParams pagingParams);
        Task<ResultModel> UpdateRecipe(int id, RecipeUpdateDTO recipeDTO);
        Task<ResultModel> DeleteRecipe(int id);
        Task<ResultModel> GetRecipesByName(string keyword, PaginationParams pagingParams);
        Task<ResultModel> GetAllRecipes(PaginationParams paginationParams, string orderBy);
        Task<ResultModel> UnlikeRecipe(string userId, int recipeId);
        Task<ResultModel> LikeRecipe(string userId, int recipeId);
        Task<ResultModel> GetRecipeCountEveryMonth();
        Task<ResultModel> GetMostLikedRecipe();
        Task<ResultModel> GetRecipeTotalCount();
        Task<ResultModel> GetRecipeById(int id);
    }
}
