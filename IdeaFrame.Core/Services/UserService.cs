using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Core.TypesConverters;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace IdeaFrame.Core.Services
{
    public class UserService : IUserService
    {
        UserManager<ApplicationUser> userManager;
        IJwtService jwtService;

        public UserService(UserManager<ApplicationUser> userManager, IJwtService _jwtService)
        {
            this.userManager = userManager;
            this.jwtService = _jwtService;

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

        public async Task<bool>AreLoginDataCorrect(RegisterLoginDTO loginData)
        {
            loginData.Password = loginData.Password + "#";
            ApplicationUser? user=await userManager.FindByNameAsync(loginData.Login);
            if(user == null)
                return false;
            bool IsPasswordCorrect=await userManager.CheckPasswordAsync(user,loginData.Password);

            return IsPasswordCorrect;
        }
    }
}
