using MealMentor.Core.Domain.Entities;

namespace MealMentor.API.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User> GetUserById(string id);
        IQueryable<User> SearchUserByEmailOrUserName(string keyword);
        Task<User> UpdateUser(User user);
        Task SendFriendRequest(string senderId, string receiverId);
        Task AcceptFriendRequest(string senderId, string receiverId);
        Task RejectFriendRequest(string senderId, string receiverId);
        Task RemoveFriendShip(string senderId, string receiverId);
        IQueryable<User?> GetPendingFriendRequest(string userId);
        IQueryable<User?> GetFriendsList(string userId);
        Task<User> GetUserWithRecipeList(string id);
        Task AddRecipeToUser(string userId, int recipeId);
        Task RemoveRecipeFromUser(string userId, int recipeId);
        bool ExistRecipeInRecipeList(string userId, int recipeId);
        Task<int> GetNewUserCountInWeek();
        Task<(int currentWeekCount, int previousWeekCount)> GetPaidUserCountInWeek();
        Task<List<Subscription>> GetSubcriberList();
        Task<int> GetTotalUser();
        Task<double?> GetTotalRevenue();
        Task<int> GetTotalPaidUser();
    }
}
