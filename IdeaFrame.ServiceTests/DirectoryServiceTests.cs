using FluentAssertions;
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
        public async Task MoveFileItem_WithFolderWhichIsMovedToItself_ExpectToGetException()
        {
            initServicesAndMocks();
            MoveFileTimeRequestDTO moveFileTimeRequestDTO = new MoveFileTimeRequestDTO()
            {
                NewPath = "/home/name",
                Name = "name",
                Path = "/home"
            };
            setupMethodsFor_MoveFileItem_WithFolderWhichIsMovedToItself_ExpectToGetException();

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


        private void setupMethodsFor_MoveFileItem_WithFolderWhichIsMovedToItself_ExpectToGetException()
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
            setupGetFileItemFromParentDirectory(null, "home", userId, homeFileSystemItem);
            setupGetFileItemFromParentDirectory(homeFileSystemItem, "name", userId, nameFileSystemItem);
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
            setupGetFileItemFromParentDirectory(null, "home", userId, homeFileSystemItem);
            setupGetFileItemFromParentDirectory(homeFileSystemItem, "name", userId, nameFileSystemItem);
            setupGetFileItemFromParentDirectory(homeFileSystemItem, "alex", userId, alexFileSystemItem);
        }

        private void setupGetFileItemFromParentDirectory(FileSystemItem? parent, String fileItemName, Guid userId,FileSystemItem valueToReturn)
        {
            
            directoryRepositoryMock
                .Setup(x => x.GetFileItemFromParentDirectory(It.Is<FileSystemItem>(a => a == parent), It.Is<String>(a => a == fileItemName), It.Is<Guid>(k => k.ToString() == userId.ToString())))
                .ReturnsAsync(valueToReturn);
        }
    }
}
