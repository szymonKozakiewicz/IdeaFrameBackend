using FluentAssertions;
using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Enums;
using IdeaFrame.Core.Domain.Exceptions;
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
    public class DirectoryServiceTests
    {

        IDirectoryService directoryService;
        IDirectoryRepository directoryRepository;
        Mock<IDirectoryRepository> directoryRepositoryMock;
        IUserService userService;
        Mock<IUserService> userServiceMock;

        public void initServicesAndMocks()
        {
            directoryRepositoryMock=new Mock<IDirectoryRepository>();
            userServiceMock = new Mock<IUserService>();
            userService = userServiceMock.Object;
            directoryRepository = directoryRepositoryMock.Object;
            directoryService = new DirectoryService(directoryRepository,userService);
        }

        [Fact]
        public async Task AddNewFileItem_WithValidName_ExpectThatMethodAddNewFileSystemItemFromRepositoryWillBeTriggered()
        {
            initServicesAndMocks();
            FileSystemItemDTO newFolder = new FileSystemItemDTO()
            {
                Name = "newFolder",
                Path = "/",
                Type = FileItemType.FOLDER
            };
            setupDefaultVersionOfMethodsForDirectoryTest();

            //act 
            await this.directoryService.AddNewFileItem(newFolder);

            //assert
            directoryRepositoryMock.Verify(x => x.AddNewFileSystemItem(It.IsAny<FileSystemItem>()), Times.Once);
        }


        [Fact]
        public async Task AddNewFileItem_WithNotValidName_ExpectThatExceptionWillBeThrowed()
        {
            initServicesAndMocks();
            FileSystemItemDTO newFolder = new FileSystemItemDTO()
            {
                Name = "home",
                Path = "/",
                Type = FileItemType.FOLDER
            };
            setupDefaultVersionOfMethodsForDirectoryTest();

            //act 

            await FluentActions
                            .Invoking(() => this.directoryService.AddNewFileItem(newFolder))
                            .Should()
                            .ThrowAsync<FileSystemNameException>();
        }



        [Fact]
        public async Task MoveFileItem_WithFolderWhichIsMovedToItself_ExpectToGetException()
        {
            initServicesAndMocks();
            MoveFileTimeRequestDTO moveFileTimeRequestDTO = new MoveFileTimeRequestDTO()
            {
                NewPath = "/home/name",
                Name = "name",
                Path = "/home"
            };
            setupDefaultVersionOfMethodsForDirectoryTest();

            await FluentActions
                            .Invoking(() => this.directoryService.MoveFileItem(moveFileTimeRequestDTO))
                            .Should()
                            .ThrowAsync<Exception>()
                            .WithMessage("Cannot move file to itself");
        }

        [Fact]
        public async Task MoveFileItem_ToCorrectFolder_ExpectToNotGetException()
        {
            initServicesAndMocks();
            MoveFileTimeRequestDTO moveFileTimeRequestDTO = new MoveFileTimeRequestDTO()
            {
                NewPath = "/home/alex",
                Name = "name",
                Path = "/home"
            };
            setupMethodsFor_MoveFileItem_ToCorrectFolder_ExpectToNotGetException();

            await FluentActions
                            .Invoking(() => this.directoryService.MoveFileItem(moveFileTimeRequestDTO))
                            .Should()
                            .NotThrowAsync<Exception>();
        }


        private void setupDefaultVersionOfMethodsForDirectoryTest()
        {
            Guid userId = Guid.NewGuid();
            userServiceMock.Setup(x => x.GetCurrentUserId()).ReturnsAsync(userId);
            FileSystemItem homeFileSystemItem = new FileSystemItem()
            {
                Id = Guid.NewGuid(),
                Name = "home",
                Owner = null,
                OwnerId = userId,
                ParentId = null,
                Parent = null,
                Type = FileItemType.FOLDER
            };
            FileSystemItem nameFileSystemItem = new FileSystemItem()
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Owner = null,
                OwnerId = userId,
                ParentId = homeFileSystemItem.Id,
                Parent = homeFileSystemItem,
                Type = FileItemType.FOLDER
            };
            var homeDbSearch = new FileSystemItemSearchInDbDTO("home", null, FileItemType.FOLDER, userId);
            var nameDbSearch = new FileSystemItemSearchInDbDTO("name", homeFileSystemItem, FileItemType.FOLDER, userId);
            setupGetFileItemFromParentDirectory(homeDbSearch, homeFileSystemItem);
            setupGetFileItemFromParentDirectory(nameDbSearch, nameFileSystemItem);
            List<FileSystemItem> homeChildrends = new List<FileSystemItem>() { nameFileSystemItem,homeFileSystemItem };
            

            setupGetParentChildrens(homeChildrends);

        }

        private void setupGetParentChildrens(List<FileSystemItem> homeChildrends)
        {
            List<FileSystemItem> emptyList = new List<FileSystemItem>();
            directoryRepositoryMock.Setup(x => x.GetAllChildrensInFolder(It.Is<FileSystemItem>(x => x == null), It.IsAny<Guid>()))
                .ReturnsAsync(homeChildrends);

            directoryRepositoryMock.Setup(x => x.GetAllChildrensInFolder(It.Is<FileSystemItem>(x => x != null), It.IsAny<Guid>()))
                .ReturnsAsync(emptyList);
        }

        private void setupMethodsFor_MoveFileItem_ToCorrectFolder_ExpectToNotGetException() 
        {
            Guid userId = Guid.NewGuid();
            userServiceMock.Setup(x => x.GetCurrentUserId()).ReturnsAsync(userId);
            FileSystemItem homeFileSystemItem = new FileSystemItem()
            {
                Id = Guid.NewGuid(),
                Name = "home",
                Owner = null,
                OwnerId = userId,
                ParentId = null,
                Parent = null,
                Type = FileItemType.FOLDER
            };
            FileSystemItem nameFileSystemItem = new FileSystemItem()
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Owner = null,
                OwnerId = userId,
                ParentId = homeFileSystemItem.Id,
                Parent = homeFileSystemItem,
                Type = FileItemType.FOLDER
            };

            FileSystemItem alexFileSystemItem = new FileSystemItem()
            {
                Id = Guid.NewGuid(),
                Name = "alex",
                Owner = null,
                OwnerId = userId,
                ParentId = homeFileSystemItem.Id,
                Parent = homeFileSystemItem,
                Type = FileItemType.FOLDER
            };
            var homeDbSearch=new FileSystemItemSearchInDbDTO("home", null, FileItemType.FOLDER, userId);
            var nameDbSearch=new FileSystemItemSearchInDbDTO("name", homeFileSystemItem, FileItemType.FOLDER, userId);
            var alexDbSearch= new FileSystemItemSearchInDbDTO("alex", homeFileSystemItem, FileItemType.FOLDER, userId);
            setupGetFileItemFromParentDirectory(homeDbSearch, homeFileSystemItem);
            setupGetFileItemFromParentDirectory(nameDbSearch, nameFileSystemItem);
            setupGetFileItemFromParentDirectory(alexDbSearch, alexFileSystemItem);

            List<FileSystemItem> homeChildrends = new List<FileSystemItem>() { nameFileSystemItem,alexFileSystemItem };


            setupGetParentChildrens(homeChildrends);
        }

        private void setupGetFileItemFromParentDirectory(FileSystemItemSearchInDbDTO dataToSearch,FileSystemItem valueToReturn)
        {
            
            directoryRepositoryMock
                .Setup(x => x.GetFileItemFromParentDirectory(It.Is<FileSystemItemSearchInDbDTO>(a => a.Name == dataToSearch.Name)))
                .ReturnsAsync(valueToReturn);
        }
    }
}
