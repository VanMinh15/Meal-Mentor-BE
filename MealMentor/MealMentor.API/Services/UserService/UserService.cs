using MealMentor.API.Pagination;
using MealMentor.API.Repositories.PaymentRepository;
using MealMentor.API.Repositories.RecipeRepository;
using MealMentor.API.Repositories.UserRepository;
using MealMentor.Core.DTOs.ResultModel;
using MealMentor.Core.DTOs.UserDTO;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IRecipeRepository _recipeRepository;
        public UserService(IUserRepository userRepository, IPaymentRepository paymentRepository, IRecipeRepository recipeRepository)
        {
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _recipeRepository = recipeRepository;
        }

        public async Task<ResultModel> EditProfile(EditProfileRequestDTO user)
        {
            var result = new ResultModel();
            try
            {
                var existingUser = await _userRepository.GetUserById(user.Id);
                if (existingUser == null)
                {
                    result.StatusCode = StatusCodes.Status404NotFound;
                    result.IsSuccess = false;
                    result.Message = "User not found";
                    return result;
                }

                existingUser.Username = user.Username;
                existingUser.Height = user.Height;
                existingUser.Weight = user.Weight;
                existingUser.BirthDate = user.BirthDate;

                var updatedUser = await _userRepository.UpdateUser(existingUser);
                var bmi = CalculateBMI((double)updatedUser.Height, (double)updatedUser.Weight);

                var userProfileResponse = new EditProfileResponseDTO
                {
                    Username = updatedUser.Username,
                    Height = updatedUser.Height,
                    Weight = updatedUser.Weight,
                    BirthDate = updatedUser.BirthDate,
                    BMI = bmi

                };

                result.StatusCode = StatusCodes.Status200OK;
                result.IsSuccess = true;
                result.Message = "User profile updated";
                result.Data = userProfileResponse;
            }
            catch (Exception e)
            {
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.IsSuccess = false;
                result.Message = e.Message;
                result.Data = null;
            }
            return result;
        }

        private double CalculateBMI(double height, double weight)
        {
            if (height <= 0) throw new ArgumentException("Height must be greater than zero");
            if (weight <= 0) throw new ArgumentException("Weight must be greater than zero");
            return weight / (height * height);
        }

        public async Task<ResultModel> SearchUserByEmailOrUserName(string? keyword, PaginationParams pagingParams)
        {
            var result = new ResultModel();
            try
            {
                var users = _userRepository.SearchUserByEmailOrUserName(keyword ?? "");

                if (users == null || !users.Any())
                {
                    result.StatusCode = StatusCodes.Status404NotFound;
                    result.IsSuccess = false;
                    result.Message = "No users found matched";
                    return result;
                }

                var userDto = users
                    .Where(user => user != null)
                    .Select(user => new UserSearchtDTO
                    {
                        Username = user.Username,
                        Email = user.Email,
                        BirthDate = user.BirthDate
                    });

                var pagedResult = await userDto.ToPagedResultAsync(pagingParams);

                result.StatusCode = StatusCodes.Status200OK;
                result.IsSuccess = true;
                result.Message = "Users found";
                result.Data = pagedResult;
            }
            catch (Exception e)
            {
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.IsSuccess = false;
                result.Message = e.Message;
                result.Data = null;
            }
            return result;
        }


        public async Task<ResultModel> SendFriendRequest(string senderId, string receiverId)
        {
            var result = new ResultModel();
            try
            {
                await _userRepository.SendFriendRequest(senderId, receiverId);
                result.IsSuccess = true;
                result.StatusCode = StatusCodes.Status200OK;
                result.Message = "Friend request sent";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> AcceptFriendRequest(string senderId, string receiverId)
        {
            var result = new ResultModel();
            try
            {
                await _userRepository.AcceptFriendRequest(senderId, receiverId);
                result.IsSuccess = true;
                result.StatusCode = StatusCodes.Status200OK;
                result.Message = "Friend request accepted";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> RejectFriendRequest(string senderId, string receiverId)
        {
            var result = new ResultModel();
            try
            {
                await _userRepository.RejectFriendRequest(senderId, receiverId);
                result.IsSuccess = true;
                result.StatusCode = StatusCodes.Status200OK;
                result.Message = "Friend request rejected";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> RemoveFriendShip(string senderId, string receiverId)
        {
            var result = new ResultModel();
            try
            {
                await _userRepository.RemoveFriendShip(senderId, receiverId);
                result.IsSuccess = true;
                result.StatusCode = StatusCodes.Status200OK;
                result.Message = "Unfriend succesfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetPendingFriendRequest(string userId, PaginationParams pagingParams)
        {
            var result = new ResultModel();
            var pendingFriendRequests = _userRepository.GetPendingFriendRequest(userId);
            try
            {
                if (pendingFriendRequests == null || !pendingFriendRequests.Any())
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status404NotFound;
                    result.Message = "No friend requests found";
                    result.Data = null;
                    return result;
                }
                var pagedResult = await pendingFriendRequests.AsQueryable().ToPagedResultAsync(pagingParams);

                result.IsSuccess = true;
                result.StatusCode = StatusCodes.Status200OK;
                result.Data = pendingFriendRequests;
                result.Message = "Friend requests retrieved";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
                result.Data = null;
            }
            return result;
        }

        public async Task<ResultModel> GetFriendsList(string userId, PaginationParams pagingParams)
        {
            var result = new ResultModel();
            var friends = _userRepository.GetFriendsList(userId);
            try
            {
                if (friends == null || !friends.Any())
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status404NotFound;
                    result.Message = "You don't have anyfriend yet";
                    result.Data = null;
                    return result;
                }
                var pagedResult = await friends.AsQueryable().ToPagedResultAsync(pagingParams);

                result.IsSuccess = true;
                result.StatusCode = StatusCodes.Status200OK;
                result.Data = friends;
                result.Message = "Friends retrieved";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
                result.Data = null;
            }
            return result;
        }

        public async Task<ResultModel> GetUserCountInWeek()
        {
            try
            {
                var newUser = await _userRepository.GetNewUserCountInWeek();
                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "New user count in week",
                    Data = newUser
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetWeeklyPaidUserCount()
        {
            try
            {
                var (currentWeekCount, previousWeekCount) = await _userRepository.GetPaidUserCountInWeek();
                var resultData = new
                {
                    CurrentWeekCount = currentWeekCount,
                    PreviousWeekCount = previousWeekCount
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Paid user count in week",
                    Data = resultData
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetSubcriberList()
        {
            try
            {
                var subscribers = await _userRepository.GetSubcriberList();
                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Subcriber list",
                    Data = subscribers
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetTotalUser()
        {
            try
            {
                var totalUser = await _userRepository.GetTotalUser();
                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Total user count",
                    Data = totalUser
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetTotalPaidUser()
        {
            try
            {
                var totalPaidUser = await _userRepository.GetTotalPaidUser();
                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Total paid user count",
                    Data = totalPaidUser
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetTotalRevenue()
        {
            try
            {
                var totalRevenue = await _userRepository.GetTotalRevenue();
                return new ResultModel
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Total revenue",
                    Data = totalRevenue
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
