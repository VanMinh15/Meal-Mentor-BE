using MealMentor.API.Database;
using MealMentor.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace MealMentor.API.Repositories.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MealMentorDbContext _context = MealMentorDbContext.Instance;

        public async Task<User> GetAccountById(string id)
        {
            return await _context.Users
                .Include(u => u.FriendshipReceivers)
                .Include(u => u.FriendshipSenders)
                .Include(u => u.Orders)
                .Include(u => u.PlanDates)
                .Include(u => u.Recipes)
                .Include(u => u.Subscriptions)
                .Where(u => u.Id == id)
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Password = u.Password,
                    Role = u.Role,
                    Height = u.Height,
                    Weight = u.Weight,
                    BirthDate = u.BirthDate,
                    Status = u.Status,
                    RecipeList = u.RecipeList,
                    CreatedDateTime = u.CreatedDateTime,
                    FriendshipReceivers = u.FriendshipReceivers,
                    FriendshipSenders = u.FriendshipSenders,
                    Orders = u.Orders,
                    PlanDates = u.PlanDates,
                    Recipes = u.Recipes,
                    Subscriptions = u.Subscriptions
                })
                .FirstOrDefaultAsync();
        }


        public IQueryable<User> GetUserList()
        {
            return _context.Users.OrderByDescending(u => u.CreatedDateTime);
        }

        public async Task<User> RegisterUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<Recipe>> GetRecipesByUserId(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || string.IsNullOrEmpty(user.RecipeList))
            {
                return new List<Recipe>();
            }

            // Remove the brackets and split the string by commas
            var recipeIds = user.RecipeList.Trim('[', ']').Split(',')
                                .Select(id => int.Parse(id.Trim()))
                                .ToList();

            var recipes = await _context.Recipes.Where(r => recipeIds.Contains(r.Id)).ToListAsync();

            return recipes;
        }


    }
}
