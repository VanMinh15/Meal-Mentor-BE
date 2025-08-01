using MealMentor.Core.Domain.Entities;

namespace MealMentor.API.Repositories.IngredientRepository
{
    public interface IIngredientRepository
    {
        Task<Ingredient> GetIngredientById(int id);
        Task<List<Ingredient>> GetIngredientList();
        Task<Ingredient> AddIngredient(Ingredient ingredient);
        Task<Ingredient> UpdateIngredient(Ingredient ingredient);
        Task DeleteIngredient(int id);
        IQueryable<Ingredient> GetIngredientByName(string keyword);
    }
}
