using MealMentor.API.Database;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MealMentor.API.Repositories.RecipeRepository
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly MealMentorDbContext _context = MealMentorDbContext.Instance;

        public async Task<Recipe> GetRecipeById(int id)
        {
            return await _context.Recipes.FindAsync(id);
        }

        public async Task<Recipe> AddRecipe(Recipe recipe)
        {
            var result = await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public IQueryable<Recipe> GetRecipeByName(string keyword)
        {
            return _context.Recipes
                .Where(r => r.Name.Contains(keyword) && r.IsDeleted == 1);
        }

        public async Task<Recipe> DeleteRecipe(int id)
        {
            var rep = await GetRecipeById(id);
            if (rep != null)
            {
                rep.IsDeleted = 0;
            }
            await _context.SaveChangesAsync();
            return rep;
        }

        public async Task<Recipe> UpdateRecipe(int id, Recipe recipe)
        {

            var rep = await GetRecipeById(id);
            if (rep != null)
            {
                rep.Ingredients = recipe.Ingredients;
                rep.Calories = recipe.Calories;
                rep.Description = recipe.Description;
                rep.Name = recipe.Name;
                rep.Preparation = recipe.Preparation;
                rep.TotalNutrients = recipe.TotalNutrients;
                rep.TotalDaily = rep.TotalDaily;
            }
            await _context.SaveChangesAsync();
            return rep;
        }
        public async Task<List<Recipe>> GetAllRecipes()
        {
            return await _context.Recipes.Where(r => r.IsDeleted == 1).ToListAsync();
        }

        public IQueryable<Recipe> GetRecipeList()
        {
            return _context.Recipes.Where(r => r.IsDeleted == 1);
        }

        public async Task<Recipe> LikeRecipe(int id)
        {
            var repcipe = await GetRecipeById(id);
            if (repcipe != null)
            {
                repcipe.LikeQuantity++;
            }
            _context.Update(repcipe);
            await _context.SaveChangesAsync();
            return repcipe;
        }

        public async Task<Recipe> DislikeRecipe(int id)
        {
            var repcipe = await GetRecipeById(id);
            if (repcipe != null)
            {
                repcipe.LikeQuantity--;
            }
            _context.Update(repcipe);
            await _context.SaveChangesAsync();
            return repcipe;
        }

        public async Task<Dictionary<string, int>> GetRecipeCountByMonth()
        {
            var recipeCounts = await _context.Recipes
                .Where(r => r.CreateDateTime.HasValue)
                .GroupBy(r => new { r.CreateDateTime.Value.Year, r.CreateDateTime.Value.Month })
                .Select(g => new
                {
                    YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Count = g.Count()
                })
                .ToListAsync();

            var allMonths = Enumerable.Range(1, 12).ToDictionary(m => m.ToString("D2"), m => 0);

            foreach (var item in recipeCounts)
            {
                var month = item.YearMonth.Split('-')[1];
                allMonths[month] = item.Count;
            }

            return allMonths;
        }

        public async Task<int> GetRecipeTotalCount()
        {
            return await _context.Recipes.Where(x => x.IsDeleted == 1).CountAsync();
        }

        public async Task<List<LikedRecipeDTO>> GetMostLikeRecipes()
        {
            var recipes = await _context.Recipes
                .OrderByDescending(r => r.LikeQuantity)
                .Where(r => r.IsDeleted == 1)
                .Take(5)
                .Select(r => new LikedRecipeDTO
                {
                    RecipeName = r.Name,
                    LikeQuantity = r.LikeQuantity ?? 0
                })
                .ToListAsync();

            return recipes;
        }


    }
}
