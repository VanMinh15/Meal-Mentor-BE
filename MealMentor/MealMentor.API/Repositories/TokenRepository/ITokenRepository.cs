using MealMentor.Core.Domain.Entities;

namespace MealMentor.API.Repositories.TokenRepository
{
    public interface ITokenRepository
    {
        Task SaveRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshToken(string token);
        Task UpdateRefreshToken(RefreshToken token);
    }
}
