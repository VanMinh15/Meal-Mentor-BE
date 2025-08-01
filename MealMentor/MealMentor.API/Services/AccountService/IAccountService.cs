using MealMentor.Core.DTOs.AccountDTO;
using MealMentor.Core.DTOs.ResultModel;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.AccountService
{
    public interface IAccountService
    {
        Task<ResultModel> GetAccountById(string id);
        Task<ResultModel> GetUserList(PaginationParams pagingParams);
        Task<ResultModel> Login(string email, string password);
        Task<ResultModel> RegisterUser(RegisterDTO RegisterDTO);

        Task<ResultModel> RefreshToken(string refreshToken);

        Task<string> FirebaseRegister(string email, string password);

    }
}
