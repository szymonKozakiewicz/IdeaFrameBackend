using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.TypesConverters
{
    public static class DtoToEntityConverter
    {
        public static ApplicationUser ConvertRegisterLoginDtoToApplicationUser(RegisterLoginDTO dto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = dto.Login

            };
            return user;
        }
    }
}
