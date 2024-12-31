using IdeaFrame.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public RefreshTokenDto(RefreshToken refreshToken)
        {
            Token = refreshToken.Token;
            Expiration = refreshToken.Expiration;

        }
    }
}
