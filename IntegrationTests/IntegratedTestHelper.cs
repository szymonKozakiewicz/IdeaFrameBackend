using IdeaFrame.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public static class IntegratedTestHelper
    {

        public static StringContent ConvertToJson<T>(T newUser)
        {
            return new StringContent(System.Text.Json.JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
        }

        public static string CreateQueryString(string url,string value,string name )
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(url);
            sb.Append("?");
            sb.Append(name);
            sb.Append("=");
            sb.Append(value);
            return sb.ToString();
        }

    }

}
