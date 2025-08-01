using MealMentor.API.Database;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MealMentor.API.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly MealMentorDbContext _context = MealMentorDbContext.Instance;

        public async Task<User> GetUserById(string id)
        {

            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserWithRecipeList(string id)
        {
            return await _context.Users
                .Include(u => u.RecipeList).FirstOrDefaultAsync(u => u.Id == id);
        }

        public IQueryable<User> SearchUserByEmailOrUserName(string keyword)
        {
            return _context.Users
                .Where(u => (u.Email.Contains(keyword) || u.Username.Contains(keyword.ToLower()))
                && u.Role == RoleEnums.Customer.ToString() && u.Status == StatusEnums.Active.ToString());
        }

        public async Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task SendFriendRequest(string senderId, string receiverId)
        {
            var friendship = new Friendship
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                RequestDate = DateTime.Now,
                Status = (int)FriendshipEnums.Pending
            };
            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();
        }

        public async Task AcceptFriendRequest(string senderId, string receiverId)
        {
            var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.SenderId == senderId
            && f.ReceiverId == receiverId
            && f.Status == (int)FriendshipEnums.Pending);

            friendship.Status = (int)FriendshipEnums.Accepted;
            friendship.ResponseDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task RejectFriendRequest(string senderId, string receiverId)
        {
            var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.SenderId == senderId
            && f.ReceiverId == receiverId
            && f.Status == (int)FriendshipEnums.Pending);

            friendship.Status = (int)FriendshipEnums.Rejected;
            friendship.ResponseDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFriendShip(string senderId, string receiverId)
        {
            var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f =>
            (f.SenderId == senderId && f.ReceiverId == receiverId)
            || (f.SenderId == receiverId && f.ReceiverId == senderId)
            && (f.Status == (int)FriendshipEnums.Accepted)
            );

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }

        public IQueryable<User?> GetPendingFriendRequest(string userId)
        {
            return _context.Friendships
                .Where(f => f.SenderId == userId && f.Status == (int)FriendshipEnums.Pending)
                .OrderByDescending(f => f.RequestDate)
                .Select(f => f.SenderId == userId ? f.Receiver : f.Sender);
        }


        public IQueryable<User?> GetFriendsList(string userId)
        {
            return _context.Friendships
                .Where(f => f.SenderId == userId && f.Status == (int)FriendshipEnums.Accepted)
                .OrderByDescending(f => f.ResponseDate)
                .Select(f => f.SenderId == userId ? f.Receiver : f.Sender);
        }

        public async Task AddRecipeToUser(string userId, int recipeId)
        {
            var user = await _context.Users.Include(u => u.Recipes).FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var recipeIds = string.IsNullOrEmpty(user.RecipeList) ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(user.RecipeList);
                if (!recipeIds.Contains(recipeId))
                {
                    recipeIds.Add(recipeId);
                    user.RecipeList = JsonConvert.SerializeObject(recipeIds);
                    await _context.SaveChangesAsync();
                }
            }
        }


        public async Task RemoveRecipeFromUser(string userId, int recipeId)
        {
            var user = await _context.Users.Include(u => u.Recipes).FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var recipeIds = string.IsNullOrEmpty(user.RecipeList) ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(user.RecipeList);
                if (recipeIds.Contains(recipeId))
                {
                    recipeIds.Remove(recipeId);
                    user.RecipeList = JsonConvert.SerializeObject(recipeIds);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public bool ExistRecipeInRecipeList(string userId, int recipeId)
        {
            var user = _context.Users.Include(u => u.Recipes).FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                var recipeIds = string.IsNullOrEmpty(user.RecipeList) ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(user.RecipeList);
                return recipeIds.Contains(recipeId);
            }
            return false;
        }

        public async Task<int> GetNewUserCountInWeek()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var newUserCount = await _context.Users
                .Where(u => u.CreatedDateTime >= startOfWeek && u.CreatedDateTime <= endOfWeek)
                .CountAsync();

            return newUserCount;
        }


        public async Task<(int currentWeekCount, int previousWeekCount)> GetPaidUserCountInWeek()
        {
            var today = DateTime.Today;
            var startOfCurrentWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfPreviousWeek = startOfCurrentWeek.AddDays(-7);
            var endOfCurrentWeek = startOfCurrentWeek.AddDays(7).AddTicks(-1);
            var endOfPreviousWeek = startOfCurrentWeek.AddTicks(-1);

            var currentWeekCount = await _context.Orders
                .Where(o => o.CreatedDateAt >= startOfCurrentWeek && o.CreatedDateAt <= endOfCurrentWeek && o.Status == PaymentEnums.Paid.ToString())
                .CountAsync();

            var previousWeekCount = await _context.Orders
                .Where(o => o.CreatedDateAt >= startOfPreviousWeek && o.CreatedDateAt <= endOfPreviousWeek && o.Status == PaymentEnums.Paid.ToString())
                .CountAsync();

            return (currentWeekCount, previousWeekCount);
        }

        public async Task<List<Subscription>> GetSubcriberList()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        public async Task<int> GetTotalUser()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalPaidUser()
        {
            return await _context.Orders
                .Where(o => o.Status == PaymentEnums.Paid.ToString())
                .CountAsync();
        }

        public async Task<double?> GetTotalRevenue()
        {
            return await _context.Orders
                .Where(o => o.Status == PaymentEnums.Paid.ToString())
                .SumAsync(o => o.Price);
        }
    }


}

