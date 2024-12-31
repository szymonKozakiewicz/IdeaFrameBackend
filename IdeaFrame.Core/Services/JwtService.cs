using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtRepository _jwtRepository;

        public JwtService(IConfiguration configuration, IJwtRepository _jwtRepository)
        {
            _configuration = configuration;
            this._jwtRepository = _jwtRepository;
        }

        public async Task<JwtResponse> CreateJwtResponse(string userName)
        {
            DateTime tokenExpires = getTokenExpiration("JWT:tokenExpirationTimeHours");

            string tokenStr = generateJwtToken(userName, tokenExpires);



            JwtResponse response = new JwtResponse()
            {
                AcessToken = tokenStr,
                AcessTokenExpiration = tokenExpires,

            };
            return response;
        }

        public async Task<RefreshTokenDto>CreateRefreshToken(string userName)
        {
            DateTime tokenRefreshExpires = getTokenExpiration("JWT:refreshTokenExpirationTimeHours");
            string refreshTokenStr = generateRefreshToken();
            RefreshToken refreshToken= await saveRefreshToken(userName, tokenRefreshExpires, refreshTokenStr);
            RefreshTokenDto refreshTokenDto = new RefreshTokenDto(refreshToken);
            return refreshTokenDto;

        }

        private async Task<RefreshToken> saveRefreshToken(string userName, DateTime tokenRefreshExpires, string refreshTokenStr)
        {
            var refreshToken = new RefreshToken()
            {
                Token = refreshTokenStr,
                Expiration = tokenRefreshExpires,
                UserName = userName
            };

            await this._jwtRepository.UpdateRefreshToken(refreshToken);
            return refreshToken;
        }


        private DateTime getTokenExpiration(string pathToTokenExpirationInConf)
        {
            double tokenExpirationTime = double.Parse(_configuration[pathToTokenExpirationInConf]);
            var tokenExpires = DateTime.Now.AddHours(tokenExpirationTime);
            return tokenExpires;
        }

        private string generateJwtToken(string userName, DateTime tokenExpires)
        {
            var claims = new[]
                        {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSymmetricalKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: tokenExpires,
                signingCredentials: creds
            );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenStr;
        }

        private string generateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
