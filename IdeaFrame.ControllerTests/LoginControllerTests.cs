using FluentAssertions;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaFrame.ControllerTests
{
    public class LoginControllerTests
    {
        IUserService userService;
        IJwtService jwtService;
        Mock<IUserService> userServiceMock;
        Mock<IJwtService> jwtServiceMock;
        LoginController loginController;

        public async Task Login_forIncorrectLoginData_expectThatItWillReturnUnauthorizedResult()
        {
            //arrange
            initMockedServicesAndController();
            RegisterLoginDTO loginDTO = new RegisterLoginDTO()
            {
                Login = "testLogin",
                Password = "Password2"
            };
            userServiceMock.Setup(userService => userService.AreLoginDataCorrect(It.IsAny<RegisterLoginDTO>()))
                .ReturnsAsync(false);

            //act
            IActionResult result =await loginController.Login(loginDTO);
            result.Should().BeOfType<UnauthorizedResult>();

        }

        private void initMockedServicesAndController()
        {
            userServiceMock = new Mock<IUserService>();
            userService = userServiceMock.Object;
            jwtServiceMock = new Mock<IJwtService>();
            jwtService = jwtServiceMock.Object;
            loginController = new LoginController(userService, jwtService);
        }
    }
}
