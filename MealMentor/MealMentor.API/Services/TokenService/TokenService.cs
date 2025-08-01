using MealMentor.API.Repositories.TokenRepository;
using MealMentor.Core.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MealMentor.API.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IConfiguration _config;
        private readonly JsonSerializerOptions _jsonOptions;

        public TokenService(ITokenRepository tokenRepository, IConfiguration config)
        {
            _tokenRepository = tokenRepository;
            _config = config;
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        }),
                Expires = DateTime.Now.AddHours(12),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var encodeToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(encodeToken);
        }


        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task SaveRefreshToken(string userId, string refreshToken)
        {
            var token = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpirationDate = DateTime.Now.AddDays(7),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            await _tokenRepository.SaveRefreshToken(token);
        }

        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            return await _tokenRepository.GetRefreshToken(token);
        }

        public async Task UpdateRefreshToken(RefreshToken token)
        {
            await _tokenRepository.UpdateRefreshToken(token);
        }

        public string SerializeObject(object obj)
        {
            return JsonSerializer.Serialize(obj, _jsonOptions);
        }
    }
}
