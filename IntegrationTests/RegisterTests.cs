using FluentAssertions;
using IdeaFrame.Core.DTO;
using IdeaFrame.Infrastructure.DbContextCustom;

namespace IntegrationTests
{
    public class RegisterTests : IClassFixture<InMemoryWebApplicationFactory>
    {
        InMemoryWebApplicationFactory clientFactory;
        HttpClient client;
        MyDbContexSqlServer dbContext;

        public RegisterTests(InMemoryWebApplicationFactory factory)
        {
            clientFactory = factory;
            this.client = this.clientFactory.CreateClient();
            dbContext=factory.GetDbContextInstance();
        }

        [Fact]
        public async Task RegisterNewUser_forTakenLoginAndValidPassword_expectThatItWillReturnBadRequest()
        {
            RegisterLoginDTO newUser = new RegisterLoginDTO()
            {
                Login = "takenLogin",
                Password = "Password2"
            };
            this.dbContext.ClearDatabaseBeforeTests();
            var newUserJson = IntegratedTestHelper.ConvertToJson(newUser);
            var firstResponse=await this.client.PostAsync("/api/Register/registerNewUser", newUserJson);

            //act
            var result = await this.client.PostAsync("/api/Register/registerNewUser", newUserJson);


            //assert
            firstResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }


        [Theory]
        [InlineData("Pa2")]
        [InlineData("Password")]
        [InlineData("password2")]
        public async Task RegisterNewUser_forInValidPasswordAndValidLogin_expectThatItWillReturnBadRequest(string password)
        {
            RegisterLoginDTO newUser = new RegisterLoginDTO()
            {
                Login = "login",
                Password = password
            };
            this.dbContext.ClearDatabaseBeforeTests();
            var newUserJson = IntegratedTestHelper.ConvertToJson(newUser);


            //act
            var result = await this.client.PostAsync("/api/Register/registerNewUser", newUserJson);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }



        [Fact]
        public async Task RegisterNewUser_forValidInput_expectThatItWillOK()
        {
            RegisterLoginDTO newUser = new RegisterLoginDTO()
            {
                Login = "takenLogin",
                Password = "Password2"
            };
            this.dbContext.ClearDatabaseBeforeTests();
            var newUserJson = IntegratedTestHelper.ConvertToJson(newUser);


            //act
            var result = await this.client.PostAsync("/api/Register/registerNewUser", newUserJson);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }



        [Fact]
        public async Task IsLoginAvailable_forNotExistingLogin_expectServerWilReturnTrue()
        {
            string login = "Login";
            string url = "/api/Register/isLoginAvailable";
            string urlWithQueryString = IntegratedTestHelper.CreateQueryString(url, login, "login");
            this.dbContext.ClearDatabaseBeforeTests();



            //act
            var result = await this.client.GetAsync(urlWithQueryString);
            string responseBody = await result.Content.ReadAsStringAsync();
            

            //assert
            responseBody.Should().Be("true");
        }


        [Fact]
        public async Task IsLoginAvailable_forExistingLogin_expectServerWilReturnFalse()
        {
            RegisterLoginDTO newUser = new RegisterLoginDTO()
            {
                Login = "takenLogin",
                Password = "Password2"
            };
            string login = "takenLogin";
            string url = "/api/Register/isLoginAvailable";
            string urlWithQueryString = IntegratedTestHelper.CreateQueryString(url, login,"login");
            this.dbContext.ClearDatabaseBeforeTests();
            var newUserJson = IntegratedTestHelper.ConvertToJson(newUser);
            var firstResponse = await this.client.PostAsync("/api/Register/registerNewUser", newUserJson);


            //act
            var result = await this.client.GetAsync(urlWithQueryString);
            string responseBody = await result.Content.ReadAsStringAsync();


            //assert
            responseBody.Should().Be("false");
        }

    }
}
