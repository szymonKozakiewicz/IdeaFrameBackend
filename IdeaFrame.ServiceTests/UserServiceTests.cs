using FluentAssertions;
using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Core.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.ServiceTests
{
    public class UserServiceTests
    {
        UserService userService;
        UserManager<ApplicationUser> userManager;
        Mock<UserManager<ApplicationUser>> userManagerMock;
        IJwtService jwtService;
        Mock<IJwtService> jwtServiceMock;
        Mock<IUserStore<ApplicationUser>> store;


        private void initServicesAndMocks()
        {
            store = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
            jwtServiceMock = new Mock<IJwtService>();
            userManager = userManagerMock.Object;
            jwtService = jwtServiceMock.Object;
            userService = new UserService(userManager, jwtService);
        }

        [Fact]
        public async Task AddNewUser_WithValidData_TriggerCreateAsyncMethodFromUSerManager()
        {
            initServicesAndMocks();
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));
            RegisterLoginDTO newUserDTO = new RegisterLoginDTO()
            {
                Login = "login",
                Password = "password"
            };

            //act
            await userService.AddNewUser(newUserDTO);

            //assert
            userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);



        }

        [Fact]
        public async Task IsLoginAvailable_ForNotExistingLogin_ExpectThatItWillReturnTrue()
        {
            initServicesAndMocks();
            ApplicationUser? user = new ApplicationUser();
            string login = "login";
            user.UserName = login;
            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            

            //act
            var result=await userService.IsLoginAvailable(login);

            //assert
            result.Should().BeFalse();
        }



        [Fact]
        public async Task IsLoginAvailable_WithExistingLogin_ExpectThatItWillReturnFalse()
        {
            initServicesAndMocks();
            ApplicationUser? user = new ApplicationUser();
            string login = "login";
            user.UserName = login;
            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);



            //act
            var result = await userService.IsLoginAvailable(login);

            //assert
            result.Should().BeFalse();
        }


        [Fact]
        public async Task AreLoginDataCorrect_WhenLoginDataCorrect_ExpectThatItWillReturnTrue()
        {
            initServicesAndMocks();
            ApplicationUser? user = new ApplicationUser();
            string login = "login";
            string password = "password";
            user.UserName = login;
            
            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            RegisterLoginDTO loginData = new RegisterLoginDTO()
            {
                Login = login,
                Password = password
            };

            //act
            var result = await userService.AreLoginDataCorrect(loginData);

            //assert
            result.Should().BeTrue();
        }


        [Fact]
        public async Task AreLoginDataCorrect_WhenLoginInCorrectAndPasswordCorrect_ExpectThatItWillReturnFalse()
        {
            initServicesAndMocks();
            ApplicationUser? user = new ApplicationUser();
            string login = "login";
            string password = "password";
            user.UserName = login;

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            RegisterLoginDTO loginData = new RegisterLoginDTO()
            {
                Login = login,
                Password = password
            };

            //act
            var result = await userService.AreLoginDataCorrect(loginData);

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AreLoginDataCorrect_WhenLoginCorrectAndPasswordIncorrect_ExpectThatItWillReturnFalse()
        {
            initServicesAndMocks();
            ApplicationUser? user = new ApplicationUser();
            string login = "login";
            string password = "password";
            user.UserName = login;

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            RegisterLoginDTO loginData = new RegisterLoginDTO()
            {
                Login = login,
                Password = password
            };

            //act
            var result = await userService.AreLoginDataCorrect(loginData);

            //assert
            result.Should().BeFalse();
        }
    }
}
