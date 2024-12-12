using FluentAssertions;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.UI.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.ControllerTests
{
    
    public class RegisterLoginControllerTests
    {
        IUserService userService;
        Mock<IUserService> userServiceMock;
        RegisterLoginController registerLoginController;


        public RegisterLoginControllerTests()
        {
            initControllerAndService();
        }

        private void initControllerAndService()
        {
            userServiceMock = new Mock<IUserService>();
            userService = userServiceMock.Object;
            registerLoginController = new RegisterLoginController(userServiceMock.Object);
        }

        [Fact]
        public async Task RegisterNewUser_forValidData_expectThatItWillReturnOkStatusAndTriggerAddNewUserMethodFromService()
        {
            initControllerAndService();
            RegisterLoginDTO newUser = new RegisterLoginDTO()
            {
                Login = "testLogin",
                Password = "Password2"
            };
            IdentityResult identityResult=IdentityResult.Success;

            userServiceMock.Setup(userService => userService.AddNewUser(It.IsAny<RegisterLoginDTO>()))
                .ReturnsAsync(identityResult);
            userServiceMock.Setup(userService => userService.IsLoginAvailable(It.IsAny<string>()))
                .ReturnsAsync(true);

            //act
            IActionResult result=await registerLoginController.RegisterNewUser(newUser);
          
            
            //assert
            Assert.NotNull(result);
            result.Should().BeOfType<OkResult>();
            userServiceMock.Verify(x=>x.AddNewUser(It.IsAny<RegisterLoginDTO>()),Times.Once);

        }

        [Fact]
        public async Task RegisterNewUser_forValidPasswordAndTakenLogin_expectThatItWillReturnBadRequest()
        {
            initControllerAndService();
            RegisterLoginDTO newUser = new RegisterLoginDTO()
            {
                Login = "takenLogin",
                Password = "Password2"
            };
            
            IdentityResult identityResult = IdentityResult.Success;
            userServiceMock.Setup(userService => userService.AddNewUser(It.IsAny<RegisterLoginDTO>()))
                .ReturnsAsync(identityResult);
            userServiceMock.Setup(userService => userService.IsLoginAvailable(It.IsAny<string>()))
                  .ReturnsAsync(false);

            //act
            var result=await registerLoginController.RegisterNewUser(newUser);

            //assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task IsLoginAvailable_forAvailableLogin_expectThatItWillReturnTrueInBody()
        {
            initControllerAndService();
            string availableLogin = "login";

            userServiceMock.Setup(userService => userService.IsLoginAvailable(It.IsAny<string>()))
                  .ReturnsAsync(true);

            //act
            var result = await registerLoginController.IsLoginAvailable(availableLogin);

            //assert
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult okResult=result as OkObjectResult;
            bool? value=okResult.Value as bool?;
            value.Should().Be(true);
        }

        [Fact]
        public async Task IsLoginAvailable_forNotAvailableLogin_expectThatItWillReturnTrueInBody()
        {
            initControllerAndService();
            string availableLogin = "login";

            userServiceMock.Setup(userService => userService.IsLoginAvailable(It.IsAny<string>()))
                  .ReturnsAsync(false);

            //act
            var result = await registerLoginController.IsLoginAvailable(availableLogin);

            //assert
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult okResult = result as OkObjectResult;
            bool? value = okResult.Value as bool?;
            value.Should().Be(false);
        }

    }
}
