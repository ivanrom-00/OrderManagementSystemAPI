using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Controllers;
using OrderService.Interfaces;
using OrderService.Models;
using Xunit;

namespace OrderService.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockTokenService = new Mock<ITokenService>();
            _authController = new AuthController(_mockTokenService.Object);
        }

        [Theory]
        [InlineData("admin", "admin", "test_token")]
        public void Login_ValidCredentials_ReturnsOkResultWithToken(string username, string password, string expectedToken)
        {
            // Set mock and models expectations
            var loginRequest = new LoginRequest { Username = username, Password = password };
            _mockTokenService.Setup(service => service.GenerateToken(It.IsAny<string>(), It.IsAny<string>())).Returns(expectedToken);

            // Act
            var result = _authController.Login(loginRequest) as OkObjectResult;

            // Assertions
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            var value = result.Value as LoginResponse;
            Assert.NotNull(value);
            Assert.Equal(expectedToken, value.Token);
        }

        [Theory]
        [InlineData("user1", "123")]
        [InlineData("user2", "123")]
        public void Login_InvalidCredentials_ReturnsUnauthorizedResult(string username, string password)
        {
            // Set model
            var loginRequest = new LoginRequest { Username = username, Password = password };

            // Act
            var result = _authController.Login(loginRequest) as UnauthorizedResult;

            // Assertions
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        }
    }
}
