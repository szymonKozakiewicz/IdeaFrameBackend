using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.Entities
{
    public class RefreshToken
    {
       public string Token { get; set; }
       public DateTime Expiration { get; set; }

        [Key]
       public string UserName { get; set; }
        public void  Update(RefreshToken refreshToken)
        {
            this.Token = refreshToken.Token;
            this.Expiration = refreshToken.Expiration;
            this.UserName = refreshToken.UserName;
        }
    }
}
