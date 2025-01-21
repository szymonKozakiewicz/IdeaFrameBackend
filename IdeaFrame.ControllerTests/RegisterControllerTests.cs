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
    
    public class RegisterControllerTests
    {
        IUserService userService;
        IJwtService jwtService;
        Mock<IUserService> userServiceMock;
        Mock<IJwtService> jwtServiceMock;
        RegisterController registerController;


        public RegisterControllerTests()
        {
            initControllerAndService();
        }

        private void initControllerAndService()
        {
            userServiceMock = new Mock<IUserService>();
            userService = userServiceMock.Object;
            jwtServiceMock = new Mock<IJwtService>();
            jwtService = jwtServiceMock.Object;
            registerController = new RegisterController(userService);
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
            IActionResult result=await registerController.RegisterNewUser(newUser);
          
            
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
            var result=await registerController.RegisterNewUser(newUser);

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
            var result = await registerController.IsLoginAvailable(availableLogin);

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
            var result = await registerController.IsLoginAvailable(availableLogin);

            //assert
            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult okResult = result as OkObjectResult;
            bool? value = okResult.Value as bool?;
            value.Should().Be(false);
        }

        

    }
}
