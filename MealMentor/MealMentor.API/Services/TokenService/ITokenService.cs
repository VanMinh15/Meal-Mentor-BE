using MealMentor.Core.Domain.Entities;

namespace MealMentor.API.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        Task SaveRefreshToken(string userId, string refreshToken);
        Task<RefreshToken> GetRefreshToken(string token);
        Task UpdateRefreshToken(RefreshToken token);
    }
}
