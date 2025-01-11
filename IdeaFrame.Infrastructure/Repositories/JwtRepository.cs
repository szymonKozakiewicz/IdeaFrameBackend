using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Infrastructure.DbContextCustom;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Infrastructure.Repositories
{
    public class JwtRepository : IJwtRepository
    {
        private MyDbContexSqlServer _context;
        public JwtRepository(MyDbContexSqlServer _context)
        {
            this._context = _context;
        }

        public async Task<RefreshToken?> FindRefreshToken(string token)
        {
            return await this._context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateRefreshToken(RefreshToken refreshToken)
        {
            
         
            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserName == refreshToken.UserName);
            if (existingToken != null)
            {
                existingToken.Update(refreshToken);
            }
            else
            {
                await this._context.RefreshTokens.AddAsync(refreshToken);
            }
            await _context.SaveChangesAsync();
    
        }
    }
}
