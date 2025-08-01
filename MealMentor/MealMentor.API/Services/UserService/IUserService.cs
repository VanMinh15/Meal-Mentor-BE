using MealMentor.Core.DTOs.ResultModel;
using MealMentor.Core.DTOs.UserDTO;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.UserService
{
    public interface IUserService
    {
        Task<ResultModel> EditProfile(EditProfileRequestDTO user);
        Task<ResultModel> SearchUserByEmailOrUserName(string keyword, PaginationParams pagingParams);
        Task<ResultModel> SendFriendRequest(string senderId, string receiverId);
        Task<ResultModel> AcceptFriendRequest(string senderId, string receiverId);
        Task<ResultModel> RejectFriendRequest(string senderId, string receiverId);
        Task<ResultModel> RemoveFriendShip(string senderId, string receiverId);
        Task<ResultModel> GetPendingFriendRequest(string userId, PaginationParams pagingParams);
        Task<ResultModel> GetFriendsList(string userId, PaginationParams pagingParams);
        Task<ResultModel> GetUserCountInWeek();
        Task<ResultModel> GetWeeklyPaidUserCount();
        Task<ResultModel> GetSubcriberList();
        Task<ResultModel> GetTotalUser();
        Task<ResultModel> GetTotalRevenue();
        Task<ResultModel> GetTotalPaidUser();
    }
}
