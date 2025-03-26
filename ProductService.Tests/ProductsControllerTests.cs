using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Controllers;
using ProductService.Interfaces;
using ProductService.Models;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProductService.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
        }

        [Theory]
        [InlineData("TestProduct 101", 4.5, 18)]
        [InlineData("TestProduct 102", 8.6, 32)]
        public async Task CreateProduct_ValidProduct_ReturnsOk(string name, decimal price, int stock)
        {
            // Set mock and models expectations
            var product = new Product { Name = name, Price = price, Stock = stock };
            _mockProductService.Setup(service => service.AddProductAsync(product)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateProduct(product);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Product>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(product, apiResponse.Data);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOk()
        {
            // Set mock and models expectations
            var products = new List<Product> { new Product { Name = "Product1", Price = 10.5m, Stock = 10 } };
            _mockProductService.Setup(service => service.GetAllProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<Product>>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(products, apiResponse.Data);
        }

        [Fact]
        public async Task GetProductById_ProductExists_ReturnsOk()
        {
            // Set mock and models expectations
            var product = new Product { Id = 1, Name = "Product", Price = 10.5m, Stock = 10 };
            _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductById(1);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Product>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(product, apiResponse.Data);
        }

        [Fact]
        public async Task GetProductById_ProductDoesNotExist_ReturnsNotFound()
        {
            // Set mock and models expectations
            _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProductById(1);

            // Assertions
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Product>>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_ValidProduct_ReturnsOk()
        {
            // Set mock and models expectations
            var product = new Product { Id = 1, Name = "UpdatedProduct", Price = 10.5m, Stock = 10 };
            _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync(product);
            _mockProductService.Setup(service => service.UpdateProductAsync(product)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProduct(1, product);

            // Assertions
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Product>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(product, apiResponse.Data);
        }

        [Fact]
        public async Task UpdateProduct_ProductDoesNotExist_ReturnsNotFound()
        {
            // Set mock and models expectations
            var product = new Product { Id = 1, Name = "UpdatedProduct", Price = 10.5m, Stock = 10 };
            _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.UpdateProduct(1, product);

            // Assertions
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Product>>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal(StatusCodes.Status404NotFound, apiResponse.StatusCode);
        }
    }
}
