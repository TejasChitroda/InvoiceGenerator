using System.Collections.Generic;
using System.Threading.Tasks;
using Invoice_Generator.Controllers;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InvoiceGenerator.Test.Controllers
{
    public class ProductControllerTest
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductController _controller;

        public ProductControllerTest()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllProduct_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product> { new Product { Id = 1, Name = "Test" } };
            _mockService.Setup(s => s.GetAllProductAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProduct();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(products, okResult.Value);
        }

        [Fact]
        public async Task GetProductById_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test" };
            _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProductById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddProduct_ReturnsOkResult_WhenProductIsValid()
        {
            // Arrange
            var dto = new ProductDto { Name = "Test", Description = "Desc", CategoryId = 1, TaxPercentage = 5 };

            // Act
            var result = await _controller.AddProduct(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
            _mockService.Verify(s => s.AddProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task AddProduct_ReturnsBadRequest_WhenProductIsNull()
        {
            // Act
            var result = await _controller.AddProduct(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product cannot be null", badRequest.Value);
        }

        [Fact]
        public async Task UpdateProductById_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var dto = new ProductDto { Name = "Test", Description = "Desc", CategoryId = 1, TaxPercentage = 5 };
            _mockService.Setup(s => s.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProductById(1, dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateProductById_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var dto = new ProductDto { Name = "Test", Description = "Desc", CategoryId = 1, TaxPercentage = 5 };
            _mockService.Setup(s => s.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateProductById(1, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProductById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var dto = new ProductDto { Name = "Test", Description = "Desc", CategoryId = 1, TaxPercentage = 5 };

            // Act
            var result = await _controller.UpdateProductById(0, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product data is invalid", badRequest.Value);
        }

        [Fact]
        public async Task DeleteProductById_ReturnsNoContent_WhenDeleteSucceeds()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProductById(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProductById_ReturnsNotFound_WhenDeleteFails()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProductById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
