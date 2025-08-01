using FirebaseAdmin.Auth;
using MealMentor.API.Pagination;
using MealMentor.API.Repositories.AccountRepository;
using MealMentor.API.Repositories.UserRepository;
using MealMentor.API.Services.Sercurity;
using MealMentor.API.Services.TokenService;
using MealMentor.Core.Domain.Entities;
using MealMentor.Core.Domain.Enums;
using MealMentor.Core.DTOs;
using MealMentor.Core.DTOs.AccountDTO;
using MealMentor.Core.DTOs.ResultModel;
using MealMentor.Core.DTOs.UserDTO;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        public AccountService(IAccountRepository accountRepository, PasswordHasher passwordHasher, ITokenService tokenService, IUserRepository userRepository)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        public async Task<ResultModel> GetAccountById(string id)
        {
            var user = await _accountRepository.GetAccountById(id);
            if (user == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Data = null
                };
            }

            var likedRecipes = await _accountRepository.GetRecipesByUserId(id);

            var userDto = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Height = user.Height,
                Weight = user.Weight,
                BirthDate = user.BirthDate,
                Status = user.Status,
                RecipeList = user.RecipeList,
                CreatedDateTime = user.CreatedDateTime,
                LikedRecipes = likedRecipes.Select(r => new RecipeDTO
                {
                    Name = r.Name,
                    TranslatedName = r.TranslatedName,
                    Description = r.Description,
                    Image = r.Image,
                }).ToList()
            };

            return new ResultModel
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "User retrieved successfully",
                Data = userDto
            };
        }

        public async Task<ResultModel> Login(string email, string password)
        {
            var result = new ResultModel();
            try
            {
                var user = await _accountRepository.GetUserByEmail(email);
                if (user != null && _passwordHasher.VerifyPassword(password, user.Password))
                {
                    var accessToken = _tokenService.GenerateJwtToken(user);
                    var refreshToken = _tokenService.GenerateRefreshToken();
                    await _tokenService.SaveRefreshToken(user.Id, refreshToken);

                    result.IsSuccess = true;
                    result.StatusCode = StatusCodes.Status200OK;
                    result.Message = "Login successfully";
                    result.Data = new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }
                else
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status400BadRequest;
                    result.Message = "Wrong email or password";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }



        public async Task<ResultModel> RefreshToken(string refreshToken)
        {
            var result = new ResultModel();
            try
            {
                var token = await _tokenService.GetRefreshToken(refreshToken);
                if (token == null || token.ExpirationDate <= DateTime.UtcNow || token.IsRevoked)
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status401Unauthorized;
                    result.Message = "Invalid or expired refresh token";
                    return result;
                }

                var user = await _accountRepository.GetAccountById(token.UserId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status404NotFound;
                    result.Message = "User not found";
                    return result;
                }

                var newAccessToken = _tokenService.GenerateJwtToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                await _tokenService.SaveRefreshToken(user.Id, newRefreshToken);

                token.IsRevoked = true;
                token.UpdatedDate = DateTime.UtcNow;
                await _tokenService.UpdateRefreshToken(token);

                result.IsSuccess = true;
                result.StatusCode = StatusCodes.Status200OK;
                result.Message = "Token refreshed successfully";
                result.Data = new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> RegisterUser(RegisterDTO registerDTO)
        {
            var result = new ResultModel();
            try
            {
                var existingEmail = await _accountRepository.GetUserByEmail(registerDTO.Email);
                var hashedPassword = _passwordHasher.HashPassword(registerDTO.Password);
                var firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    Email = registerDTO.Email,
                    Password = registerDTO.Password,
                    EmailVerified = false
                });
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(firebaseUser.Uid, new Dictionary<string, object>
            {
                { "Customer", true },
            });

                if (existingEmail != null)
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status409Conflict;
                    result.Message = "Email already exists";
                    return result;
                }

                var user = new User
                {
                    Id = firebaseUser.Uid,
                    Email = registerDTO.Email,
                    Password = hashedPassword,
                    Username = registerDTO.UserName,
                    BirthDate = null,
                    Status = StatusEnums.Active.ToString(),
                    Role = "Customer",
                    CreatedDateTime = DateTime.Now
                };
                var newUser = await _accountRepository.RegisterUser(user);

                if (newUser != null)
                {
                    result.IsSuccess = true;
                    result.StatusCode = StatusCodes.Status200OK;
                    result.Message = "Register successfully";
                    result.Data = newUser;
                }
                else
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status400BadRequest;
                    result.Message = "Register failed";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<string> FirebaseRegister(string email, string password)
        {
            UserRecord user = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                Email = email,
                Password = password,
            });
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Uid, new Dictionary<string, object>
            {
                { "Customer", true },
            });
            return user.Uid;
        }

        public async Task<ResultModel> GetUserList(PaginationParams pagingParams)
        {
            var result = new ResultModel();
            try
            {
                var users = _accountRepository.GetUserList();
                if (users != null)
                {
                    result.IsSuccess = true;
                    result.StatusCode = StatusCodes.Status200OK;
                    result.Message = "Users found";
                    var pagedResult = await users.AsQueryable().ToPagedResultAsync(pagingParams);

                    result.Data = users;
                }
                else
                {
                    result.IsSuccess = false;
                    result.StatusCode = StatusCodes.Status404NotFound;
                    result.Message = "No User found";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}

