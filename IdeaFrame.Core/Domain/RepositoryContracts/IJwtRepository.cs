using IdeaFrame.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.RepositoryContracts
{
    public interface IJwtRepository
    {
        public Task UpdateRefreshToken(RefreshToken refreshToken);
    }
}
