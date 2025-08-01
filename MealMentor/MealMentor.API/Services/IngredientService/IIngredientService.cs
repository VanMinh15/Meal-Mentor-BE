using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs.ResultModel;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.IngredientService
{
    public interface IIngredientService
    {
        Task<ResultModel> GetIngredients(string? searchTerm, List<string>? whitelist, List<string>? blackList, PaginationParams pagingParams);
        Task<ResultModel> UpdateIngredient(int id, string? description, string? name, string translatedName, string? image);
        Task<ResultModel> DeleteIngredient(int id);
        Task<ResultModel> AddIngredient(Ingredient ingredient);
    }
}
