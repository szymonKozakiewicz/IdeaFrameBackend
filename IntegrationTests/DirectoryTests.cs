using FluentAssertions;
using IdeaFrame.Core.Domain.Enums;
using IdeaFrame.Core.DTO;
using IdeaFrame.Infrastructure.DbContextCustom;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class DirectoryTests : IClassFixture<InMemoryWebApplicationFactory>
    {
        InMemoryWebApplicationFactory clientFactory;
        HttpClient client;
        MyDbContexSqlServer dbContext;
        IConfiguration _configuration;

        public DirectoryTests(InMemoryWebApplicationFactory factory)
        {
            clientFactory = factory;
            this.client = this.clientFactory.CreateClient();
            dbContext = factory.GetDbContextInstance();
            this._configuration = factory.Configuration;
        }

        [Fact]
        public async Task AddNewFileItem_forNameWithValidNumberOfCharacters_expectToGetOkResponse()
        {

            FileSystemItemDTO newFolder = new FileSystemItemDTO()
            {
                Name = "newFolder",
                Path = "/",
                Type = FileItemType.FOLDER
            };

            dbContext.ClearDatabaseBeforeTests();

            await IntegratedTestHelper.RegisterAndLoginAsDefaultUser(this.client, this._configuration);
            var newFolderJson = IntegratedTestHelper.ConvertToJson(newFolder);

            //act
            var result = await this.client.PostAsync("/api/Directory/addNewFileItem", newFolderJson);

            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


        [Fact]
        public async Task AddNewFileItem_forNameWithNotValidNumberOfCharacters_expectToGetBadRequestResponse()

        {

            FileSystemItemDTO newFolder = new FileSystemItemDTO()
            {
                Name = "arjsopwiogkiwadmsiniwlf",
                Path = "/",
                Type = FileItemType.FOLDER
            };

            dbContext.ClearDatabaseBeforeTests();
            await IntegratedTestHelper.RegisterAndLoginAsDefaultUser(this.client, this._configuration);

            var newFolderJson = IntegratedTestHelper.ConvertToJson(newFolder);

            //act
            var result = await this.client.PostAsync("/api/Directory/addNewFileItem", newFolderJson);

            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
