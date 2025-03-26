using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Controllers;
using OrderService.Interfaces;
using OrderService.Models;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OrderService.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ILogger<OrdersController>> _mockLogger;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _controller = new OrdersController(_mockOrderService.Object, _mockLogger.Object);
        }

        [Theory]
        [InlineData(1, 1, 5, 22.5, "2025-03-24T10:30:30.333Z")]
        [InlineData(10, 3, 15, 129, "2025-03-26T10:30:30.333Z")]
        public async Task CreateOrder_ValidOrder_ReturnsOk(int customerId, int productId, int quantity, decimal totalAmount, DateTime orderDate)
        {
            // Set mock and models expectations
            var order = new Order { CustomerId = customerId, ProductId = productId, Quantity = quantity, TotalAmount = totalAmount, OrderDate = orderDate };
            _mockOrderService.Setup(service => service.AddOrderAsync(order)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateOrder(order);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(order, apiResponse.Data);
        }

        [Fact]
        public async Task CreateOrder_InvalidModel_ReturnsBadRequest()
        {
            // Set mock and models expectations
            _controller.ModelState.AddModelError("CustomerId", "Required");

            // Act
            var result = await _controller.CreateOrder(new Order());

            // Assertions
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order?>>(badRequestResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteOrder_OrderExists_ReturnsOk()
        {
            // Set mock and models expectations
            var orderId = 1;
            var order = new Order { Id = orderId };
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            _mockOrderService.Setup(service => service.DeleteOrderAsync(orderId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order?>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteOrder_OrderNotFound_ReturnsNotFound()
        {
            // Set mock and models expectations
            var orderId = 1;
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync((Order?)null);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assertions
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order?>>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
        }

        [Fact]
        public async Task GetOrderById_OrderExists_ReturnsOk()
        {
            // Set mock and models expectations
            var orderId = 1;
            var order = new Order { Id = orderId };
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(order, apiResponse.Data);
        }

        [Fact]
        public async Task GetOrderById_OrderNotFound_ReturnsNotFound()
        {
            // Set mock and models expectations
            var orderId = 1;
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync((Order?)null);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assertions
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order?>>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllOrders_ReturnsOk()
        {
            // Set mock and models expectations
            var orders = new List<Order> { new Order { Id = 1 }, new Order { Id = 2 } };
            _mockOrderService.Setup(service => service.GetAllOrdersAsync()).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetAllOrders();

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<Order>>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(orders, apiResponse.Data);
        }

        [Fact]
        public async Task UpdateOrder_ValidOrder_ReturnsOk()
        {
            // Set mock and models expectations
            var orderId = 1;
            var order = new Order { CustomerId = 1, ProductId = 1, Quantity = 1, TotalAmount = 10.0m, OrderDate = DateTime.UtcNow };
            var existingOrder = new Order { Id = orderId };
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync(existingOrder);
            _mockOrderService.Setup(service => service.UpdateOrderAsync(order)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateOrder(orderId, order);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(order, apiResponse.Data);
        }

        [Fact]
        public async Task UpdateOrder_InvalidModel_ReturnsBadRequest()
        {
            // Set mock and models expectations
            var orderId = 1;
            _controller.ModelState.AddModelError("CustomerId", "Required");

            // Act
            var result = await _controller.UpdateOrder(orderId, new Order());

            // Assertions
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order?>>(badRequestResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateOrder_OrderNotFound_ReturnsNotFound()
        {
            // Set mock and models expectations
            var orderId = 1;
            var order = new Order { CustomerId = 1, ProductId = 1, Quantity = 1, TotalAmount = 10.0m, OrderDate = DateTime.UtcNow };
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync((Order?)null);

            // Act
            var result = await _controller.UpdateOrder(orderId, order);

            // Assertions
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Order?>>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
        }
    }
}
