using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class JwtResponse
    {

        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration {  get; set; }


    }
}
