using IdeaFrame.Core.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.ServiceContracts
{
    public interface IUserService
    {
        public Task<IdentityResult> AddNewUser(RegisterLoginDTO newUser);
    }
}
