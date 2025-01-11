using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.ServiceContracts
{
    public interface IJwtService
    {
        JwtResponse CreateJwtResponse(string userName);
        public Task<RefreshTokenDto> CreateRefreshToken(string userName);

        public Task<JwtResponse> CreateJwtResponseIfTokenValid(string refreshToken);
    }
}
