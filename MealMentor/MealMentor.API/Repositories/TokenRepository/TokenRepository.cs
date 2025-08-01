using MealMentor.API.Database;
using MealMentor.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealMentor.API.Repositories.TokenRepository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly MealMentorDbContext _context = MealMentorDbContext.Instance;
        public async Task SaveRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }
        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task UpdateRefreshToken(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
