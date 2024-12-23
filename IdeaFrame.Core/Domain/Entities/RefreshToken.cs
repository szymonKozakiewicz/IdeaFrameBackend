using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.Entities
{
    public class RefreshToken
    {
       public string Token { get; set; }
       public DateTime Expiration { get; set; }
        public string UserName { get; set; }
    }
}
