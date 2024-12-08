using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Core.TypesConverters;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Services
{
    public class UserService : IUserService
    {
        UserManager<ApplicationUser> userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            
        }
        public async Task<IdentityResult> AddNewUser(RegisterLoginDTO newUserDTO)
        {
            ApplicationUser newUser = DtoToEntityConverter.ConvertRegisterLoginDtoToApplicationUser(newUserDTO);
            IdentityResult result = await this.userManager.CreateAsync(newUser, newUserDTO.Password);
            return result;
        }

        public async Task<bool> IsLoginAvailable(string login)
        {
            var user = await this.userManager.FindByNameAsync(login);
            if (user == null)
                return true;
            else
                return false;
        }
    }
}
