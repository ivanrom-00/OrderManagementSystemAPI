using CustomerService.Controllers;
using CustomerService.Interfaces;
using CustomerService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SharedLibrary.Models;

namespace CustomerService.Tests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<ILogger<CustomersController>> _mockLogger;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockLogger = new Mock<ILogger<CustomersController>>();
            _controller = new CustomersController(_mockCustomerService.Object, _mockLogger.Object);
        }

        [Theory]
        [InlineData("John", "Doe", "john.doe@example.com")]
        [InlineData("Jane", "Doe", "jane.doe@example.com")]
        public async Task CreateCustomer_ReturnsOkResult_WhenModelIsValid(string firstName, string lastName, string email)
        {
            // Set mock and models expectations
            var customer = new Customer { FirstName = firstName, LastName = lastName, Email = email };
            _mockCustomerService.Setup(service => service.AddCustomerAsync(customer)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateCustomer(customer);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Customer>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(customer, apiResponse.Data);
        }

        [Fact]
        public async Task CreateCustomer_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Set mock and models expectations
            _controller.ModelState.AddModelError("FirstName", "Required");

            // Act
            var result = await _controller.CreateCustomer(new Customer());

            // Assertions
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Customer?>>(badRequestResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllCustomers_ReturnsOkResult_WithListOfCustomers()
        {
            // Set mock and models expectations
            var customers = new List<Customer> { new Customer { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" } };
            _mockCustomerService.Setup(service => service.GetAllCustomersAsync()).ReturnsAsync(customers);

            // Act
            var result = await _controller.GetAllCustomers();

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<Customer>>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(customers, apiResponse.Data);
        }

        [Fact]
        public async Task GetCustomerById_ReturnsOkResult_WhenCustomerExists()
        {
            // Set mock and models expectations
            var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

            // Act
            var result = await _controller.GetCustomerById(1);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Customer>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(customer, apiResponse.Data);
        }

        [Fact]
        public async Task GetCustomerById_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Set mock and models expectations
            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act
            var result = await _controller.GetCustomerById(1);

            // Assertions
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Customer?>>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsOkResult_WhenCustomerIsUpdated()
        {
            // Set mock and models expectations
            var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
            _mockCustomerService.Setup(service => service.UpdateCustomerAsync(customer)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCustomer(1, customer);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Customer>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(customer, apiResponse.Data);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Set mock and models expectations
            var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act
            var result = await _controller.UpdateCustomer(1, customer);

            // Assertions
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Customer?>>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
        }
    }
}
