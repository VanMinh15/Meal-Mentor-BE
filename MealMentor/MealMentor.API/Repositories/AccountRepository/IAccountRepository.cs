using MealMentor.Core.Domain.Entities;

namespace MealMentor.API.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        Task<User> GetAccountById(string id);
        IQueryable<User> GetUserList();
        Task<User> RegisterUser(User user);
        Task<User> GetUserByEmail(string email);
        Task<List<Recipe>> GetRecipesByUserId(string userId);

    }
}
