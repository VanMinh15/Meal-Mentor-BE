using MealMentor.API.Database;
using MealMentor.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealMentor.API.Repositories.IngredientRepository
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly MealMentorDbContext _context = MealMentorDbContext.Instance;

        public async Task<Ingredient> GetIngredientById(int id)
        {
            return await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Ingredient>> GetIngredientList()
        {
            return await _context.Ingredients.Where(x => x.IsDeleted == 1).ToListAsync();
        }

        public async Task<Ingredient> AddIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<Ingredient> UpdateIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Update(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task DeleteIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
            ingredient.IsDeleted = 0;
            await _context.SaveChangesAsync();
        }

        public IQueryable<Ingredient> GetIngredientByName(string keyword)
        {
            return _context.Ingredients
                .Where(i => i.Name.Contains(keyword));
        }

    }
}
