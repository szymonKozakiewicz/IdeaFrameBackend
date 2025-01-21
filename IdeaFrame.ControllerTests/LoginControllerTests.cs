using FluentAssertions;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.UI.Controllers;
using Microsoft.AspNetCore.Http;
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

        [Fact]
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

        [Fact]
        public async Task Login_forCorrectLoginData_expectThatItWillReturnOkResultWithJwtResponseInBodyAndAddRefreshTokenToDB()
        {
            //arrange
            initMockedServicesAndController();
            RegisterLoginDTO loginDTO = new RegisterLoginDTO()
            {
                Login = "testLogin",
                Password = "Password2"
            };
            JwtResponse jwtResponse = new JwtResponse()
            {
                AccessToken = "testToken",
                AccessTokenExpiration = DateTime.Now
            };
            RefreshTokenDto refreshToken = new RefreshTokenDto("testToken", DateTime.Now);
            userServiceMock.Setup(userService => userService.AreLoginDataCorrect(It.IsAny<RegisterLoginDTO>()))
                .ReturnsAsync(true);
            jwtServiceMock.Setup(jwtService => jwtService.CreateJwtResponse(It.IsAny<string>()))
                .Returns(jwtResponse);
            jwtServiceMock.Setup(jwtService => jwtService.CreateRefreshToken(It.IsAny<string>()))
                .ReturnsAsync(refreshToken);


            //act
            IActionResult result = await loginController.Login(loginDTO);
           
            //assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeOfType<JwtResponse>();


        }

        [Fact]
        public async Task RefreshToken_ifNotValidRefreshTokenInCookies_expectThatItWillReturnUnauthorizedResult()
        {
            // arrange
            initMockedServicesAndController();
            String refreshTokenValue = "tokenValue";
            String cookieName = "refreshToken";

            loginController.ControllerContext.HttpContext.Request.Headers.Append("Cookie", $"{cookieName}={refreshTokenValue}");

            jwtServiceMock.Setup(jwtService => jwtService.CreateJwtResponseIfTokenValid(refreshTokenValue))
                .ReturnsAsync((JwtResponse)null); ;

            // act
            IActionResult result = await loginController.RefreshToken();

            // assert
            result.Should().BeOfType<UnauthorizedResult>();
        }


        [Fact]
        public async Task RefreshToken_ifNoRefreshTokenInCookies_expectThatItWillReturnUnauthorizedResult()
        {
            // arrange
            initMockedServicesAndController();


            // act
            IActionResult result = await loginController.RefreshToken();

            // assert
            result.Should().BeOfType<UnauthorizedResult>();
        }


        [Fact]
        public async Task RefreshToken_ifValidRefreshTokenInCookies_expectThatItWillReturnUnauthorizedResult()
        {
            // arrange
            initMockedServicesAndController();
            String refreshTokenValue = "tokenValue";
            String cookieName = "refreshToken";


            loginController.ControllerContext.HttpContext.Request.Headers.Append("Cookie", $"{cookieName}={refreshTokenValue}");

            jwtServiceMock.Setup(jwtService => jwtService.CreateJwtResponseIfTokenValid(refreshTokenValue))
                .ReturnsAsync(new JwtResponse { AccessToken = "newAccessToken", AccessTokenExpiration = DateTime.Now }); ;


            // act
            IActionResult result = await loginController.RefreshToken();

            // assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeOfType<JwtResponse>();
        }


        private void initMockedServicesAndController()
        {
            userServiceMock = new Mock<IUserService>();
            userService = userServiceMock.Object;
            jwtServiceMock = new Mock<IJwtService>();
            jwtService = jwtServiceMock.Object;
            loginController = new LoginController(userService, jwtService);
            loginController.ControllerContext.HttpContext = new DefaultHttpContext();
        }
    }
}
