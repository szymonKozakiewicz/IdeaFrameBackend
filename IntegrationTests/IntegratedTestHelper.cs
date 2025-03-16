using IdeaFrame.Core.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Claims;
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


        public static async Task AddNewUser(String name, HttpClient client, String password="Password2")
        {
            RegisterLoginDTO newUser = new RegisterLoginDTO()
            {
                Login = name,
                Password = password
            };
            var newUserJson = IntegratedTestHelper.ConvertToJson(newUser);

            var firstResponse = await client.PostAsync("/api/Register/registerNewUser", newUserJson);


        }


        public static async Task RegisterAndLoginAsDefaultUser(HttpClient client,IConfiguration configuration)
        {
            string userName = "default";
            string password = "Password2";
            await AddNewUser(userName, client,password);
            var loginResponse =await Login(userName,password, client);
            string token =await getJWTToken(loginResponse);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private static async Task<string> getJWTToken(HttpResponseMessage loginResponse)
        {
            string resultJSON=await loginResponse.Content.ReadAsStringAsync();
            JwtResponse jwtResponse = JsonConvert.DeserializeObject<JwtResponse>(resultJSON);
            return jwtResponse.AccessToken;
        }

        public static async Task<HttpResponseMessage> Login(string userName, string password, HttpClient client)
        {
            RegisterLoginDTO loginData = new RegisterLoginDTO()
            {
                Login = userName,
                Password = password
            };
            var loginDataJSON= ConvertToJson(loginData);
            return await client.PostAsync("/api/Login/login",loginDataJSON);
        }

    }



}
