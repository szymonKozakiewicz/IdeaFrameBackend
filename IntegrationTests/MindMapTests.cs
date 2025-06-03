using FluentAssertions;
using IdeaFrame.Core.Domain.Enums;
using IdeaFrame.Core.DTO;
using IdeaFrame.Infrastructure.DbContextCustom;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class MindMapTests : IClassFixture<InMemoryWebApplicationFactory>
    {
        InMemoryWebApplicationFactory clientFactory;
        HttpClient client;
        MyDbContexSqlServer dbContext;
        IConfiguration _configuration;
        public MindMapTests(InMemoryWebApplicationFactory factory)
        {
            clientFactory = factory;
            this.client = this.clientFactory.CreateClient();
            dbContext = factory.GetDbContextInstance();
            this._configuration = factory.Configuration;

        }

        [Fact]
        public async Task SaveMindMap_forNodesToAdd_expectToGetOkResponse()
        {
            var fileSystemItemDTO = new FileSystemItemDTO()
            {
                Name = "name",
                Path = "/",
                Type = FileItemType.FILE
            };
            SaveMindMapDTO saveDTO;
            saveDTO = createSaveDTO(fileSystemItemDTO);

            dbContext.ClearDatabaseBeforeTests();

            await IntegratedTestHelper.RegisterAndLoginAsDefaultUser(this.client, this._configuration);
            var saveMindMapJson = IntegratedTestHelper.ConvertToJson(saveDTO);
            var newFolderJson = IntegratedTestHelper.ConvertToJson(fileSystemItemDTO);
            await this.client.PostAsync("/api/Directory/addNewFileItem", newFolderJson);

            //act
            var result = await this.client.PostAsync("/api/MindMap/saveMindMap", saveMindMapJson);

            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


        [Fact]
        public async Task SaveMindMap_forNodesToUpdate_expectToGetOkResponse()
        {
            var fileSystemItemDTO = new FileSystemItemDTO()
            {
                Name = "name",
                Path = "/",
                Type = FileItemType.FILE
            };
            SaveMindMapDTO saveDTO;
            saveDTO=createSaveDTO(fileSystemItemDTO);

            dbContext.ClearDatabaseBeforeTests();

            await IntegratedTestHelper.RegisterAndLoginAsDefaultUser(this.client, this._configuration);
            var saveMindMapJson = IntegratedTestHelper.ConvertToJson(saveDTO);
            var newFolderJson = IntegratedTestHelper.ConvertToJson(fileSystemItemDTO);
            await this.client.PostAsync("/api/Directory/addNewFileItem", newFolderJson);
            var responseAfterAddNodes = await this.client.PostAsync("/api/MindMap/saveMindMap", saveMindMapJson);
            List<MindMapNodeDTO> nodesInDb = await IntegratedTestHelper.DeserializeJson<List<MindMapNodeDTO>>(responseAfterAddNodes);
            var nodeToUpdate = nodesInDb[0];
            var updatedNode = new MindMapNodeSaveDTO()
            {
                Id = nodeToUpdate.Id,
                Name = "updatedNode",
                Coordinates = new CoordinatesDTO(150, 250),
                Color = "#FF0000",
                WasEdited = true
            };
            saveDTO.Nodes.Add(updatedNode);

            //act
            var result = await this.client.PostAsync("/api/MindMap/saveMindMap", saveMindMapJson);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task LoadMindMap_forValidData_expectToGetOkResponse()
        {
            var fileSystemItemDTO = new FileSystemItemDTO()
            {
                Name = "name",
                Path = "/",
                Type = FileItemType.FILE
            };
            SaveMindMapDTO saveDTO;
            saveDTO = createSaveDTO(fileSystemItemDTO);

            dbContext.ClearDatabaseBeforeTests();

            await IntegratedTestHelper.RegisterAndLoginAsDefaultUser(this.client, this._configuration);
            var saveMindMapJson = IntegratedTestHelper.ConvertToJson(saveDTO);
            var newFolderJson = IntegratedTestHelper.ConvertToJson(fileSystemItemDTO);
            await this.client.PostAsync("/api/Directory/addNewFileItem", newFolderJson);
            await this.client.PostAsync("/api/MindMap/saveMindMap", saveMindMapJson);



            //act
            var fileSystemItemJson = IntegratedTestHelper.ConvertToJson(fileSystemItemDTO);
            var result = await this.client.PostAsync("/api/MindMap/loadMindMap", fileSystemItemJson);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        private static SaveMindMapDTO createSaveDTO(FileSystemItemDTO fileSystemItemDTO)
        {

            var nodes = new List<MindMapNodeSaveDTO>();
            var newNodeDTO = new MindMapNodeSaveDTO()
            {
                Id = "",
                Name = "node1",
                Coordinates = new CoordinatesDTO(100, 200),
                Color = "#FFFFFF",
                WasEdited = false

            };
            var editedNode = new MindMapNodeSaveDTO()
            {
                Id = "",
                Name = "node2",
                Coordinates = new CoordinatesDTO(100, 200),
                Color = "#FFFFFF",
                WasEdited = false

            };
            nodes.Add(newNodeDTO);
            nodes.Add(editedNode);

            var saveDTO = new SaveMindMapDTO()
            {
                FileItem = fileSystemItemDTO,
                Nodes = nodes
            };
            return saveDTO;
        }
    }
}
