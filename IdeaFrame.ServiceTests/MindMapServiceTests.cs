using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Enums;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.ServiceTests
{
    public class MindMapServiceTests
    {
        Mock<IDirectoryService> mockDirectoryService;
        Mock<IMindMapRepository> mockMindMapRepository;
        IDirectoryService directoryService;
        IMindMapRepository mindMapRepository;
        MindMapService mindMapService;

        public void initMocks()
        {
            mockDirectoryService = new Mock<IDirectoryService>();
            mockMindMapRepository = new Mock<IMindMapRepository>();
            directoryService = mockDirectoryService.Object;
            mindMapRepository = mockMindMapRepository.Object;
            mindMapService = new MindMapService(directoryService, mindMapRepository);

        }

        [Fact]
        public async Task GetNodesByFileId_WithValidInput_ExpectThatMethodGetNodesByFileIdFromMindMapRepositoryWillBeTriggered()
        {
            //arrange
            mockDirectoryService.Setup(x=>x.GetFileItem(It.IsAny<FileSystemItemDTO>()))
                .ReturnsAsync(new FileSystemItem()
                {
                    Id = Guid.NewGuid(),
                    Name = "name",
                    Type = FileItemType.FILE
                });
            mockMindMapRepository.Setup(x=>x.GetNodesByFileId(It.IsAny<Guid>()))
                .ReturnsAsync(new List<MindMapNode>()
                {
                    new MindMapNode()
                    {
                        Id = Guid.NewGuid(),
                        Name = "node1",
                        PositionX = 100,
                        PositionY = 200,
                        Color = "#FFFFFF"
                    },
                    new MindMapNode()
                    {
                        Id = Guid.NewGuid(),
                        Name = "node2",
                        PositionX = 300,
                        PositionY = 400,
                        Color = "#000000"
                    }
                });

            var fileSystemItemDTO = new FileSystemItemDTO()
            {
                Name = "name",
                Path = "/path/to/file",
                Type = FileItemType.FILE
            };


            //act
            await mindMapService.GetMindMap(fileSystemItemDTO);

            //assert
            mockDirectoryService.Verify(x => x.GetFileItem(It.IsAny<FileSystemItemDTO>()), Times.Once);

        }



        [Fact]
        public async Task SaveMindMap_WithValidInput_ExpectThatMethodsAddNewNodesAndUpdateNodesWillBeTriggeredWithValidArguments()
        {
            //arrange
            initMocks();
            mockDirectoryService.Setup(x => x.GetFileItem(It.IsAny<FileSystemItemDTO>()))
                .ReturnsAsync(new FileSystemItem()
                {
                    Id = Guid.NewGuid(),
                    Name = "name",
                    Type = FileItemType.FILE
                });
            mockMindMapRepository.Setup(x=>x.AddNewNodes(It.IsAny<List<MindMapNode>>()))
                .Returns(Task.CompletedTask);
            mockMindMapRepository.Setup(x => x.UpdateNodes(It.IsAny<List<MindMapNode>>()))
                .Returns(Task.CompletedTask);
            var fileSystemItemDTO=new FileSystemItemDTO()
            {
                Name = "name",
                Path = "/path/to/file",
                Type = FileItemType.FILE
            };
            var nodes = new List<MindMapNodeSaveDTO>();
            var newNodeDTO = new MindMapNodeSaveDTO()
            {
                Id = "",
                Name = "node1",
                Coordinates = new CoordinatesDTO(100, 200),
                Color = "#FFFFFF",
                WasEdited = false

            };
            var editedNode=new MindMapNodeSaveDTO()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "node2",
                Coordinates = new CoordinatesDTO(100, 200),
                Color = "#FFFFFF",
                WasEdited = true

            };
            nodes.Add(newNodeDTO);
            nodes.Add(editedNode);


            //act
            await mindMapService.SaveMindMap(new SaveMindMapDTO()
            {
                FileItem = fileSystemItemDTO,
                Nodes = nodes
            });


            //assert
            mockMindMapRepository.Verify(x => x.AddNewNodes(It.Is<List<MindMapNode>>(n => n.Count == 1 && n[0].Name == newNodeDTO.Name)), Times.Once);
            mockMindMapRepository.Verify(x => x.UpdateNodes(It.Is<List<MindMapNode>>(n => n.Count == 1 && n[0].Id == Guid.Parse(editedNode.Id) && n[0].Name == editedNode.Name)), Times.Once);


        }
    }
}
