using Invoice_Generator.Controllers;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Invoice_Generator.Tests.Controllers
{
    public class ProductPriceControllerTests
    {
        private readonly Mock<IProductPrice> _mockService;
        private readonly ProductPriceController _controller;

        public ProductPriceControllerTests()
        {
            _mockService = new Mock<IProductPrice>();
            _controller = new ProductPriceController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllPrices_ReturnsOkResult_WithPrices()
        {
            // Arrange
            var prices = new List<ProductPrice>
            {
                new ProductPrice { Id = 1, ProductId = 1, Price = 100 },
                new ProductPrice { Id = 2, ProductId = 2, Price = 200 }
            };
            _mockService.Setup(s => s.GetAllPriceAsync()).ReturnsAsync(prices);

            // Act
            var result = await _controller.GetAllPrices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnPrices = Assert.IsType<List<ProductPrice>>(okResult.Value);
            Assert.Equal(2, returnPrices.Count);
        }

        [Fact]
        public async Task GetPriceById_ReturnsOkResult_WhenPriceExists()
        {
            // Arrange
            var price = new ProductPrice { Id = 1, ProductId = 1, Price = 100 };
            _mockService.Setup(s => s.GetPriceByIdAsync(1)).ReturnsAsync(price);

            // Act
            var result = await _controller.GetPriceById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPrice = Assert.IsType<ProductPrice>(okResult.Value);
            Assert.Equal(1, returnedPrice.Id);
        }

        [Fact]
        public async Task GetPriceById_ReturnsNotFound_WhenPriceDoesNotExist()
        {
            _mockService.Setup(s => s.GetPriceByIdAsync(1)).ReturnsAsync((ProductPrice?)null);

            var result = await _controller.GetPriceById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPriceByProductId_ReturnsOkResult_WhenPriceExists()
        {
            var price = new ProductPrice { Id = 1, ProductId = 1, Price = 100 };
            _mockService.Setup(s => s.GetPriceByProductIdAsync(1)).ReturnsAsync(price);

            var result = await _controller.GetPriceByProductId(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPrice = Assert.IsType<ProductPrice>(okResult.Value);
            Assert.Equal(1, returnedPrice.ProductId);
        }

        [Fact]
        public async Task GetPriceByProductId_ReturnsNotFound_WhenPriceDoesNotExist()
        {
            _mockService.Setup(s => s.GetPriceByProductIdAsync(1)).ReturnsAsync((ProductPrice?)null);

            var result = await _controller.GetPriceByProductId(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddPrice_ReturnsOkResult_WhenValid()
        {
            var productPriceDto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 150,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = DateTime.UtcNow.AddDays(30)
            };

            _mockService.Setup(s => s.AddPriceAsync(It.IsAny<ProductPrice>()))
                        .Returns(Task.CompletedTask);

            var result = await _controller.AddPrice(productPriceDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdPrice = Assert.IsType<ProductPrice>(okResult.Value);
            Assert.Equal(productPriceDto.ProductId, createdPrice.ProductId);
            Assert.Equal(productPriceDto.Price, createdPrice.Price);
        }

        [Fact]
        public async Task AddPrice_ReturnsBadRequest_WhenDtoIsNull()
        {
            var result = await _controller.AddPrice(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product price cannot be null.", badRequest.Value);
        }

        [Fact]
        public async Task UpdatePrice_ReturnsNoContent_WhenSuccessful()
        {
            var productPriceDto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 180,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = DateTime.UtcNow.AddDays(30)
            };

            _mockService.Setup(s => s.UpdatePriceAsync(It.IsAny<ProductPrice>()))
                        .ReturnsAsync(true);

            var result = await _controller.UpdatePrice(1, productPriceDto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePrice_ReturnsNotFound_WhenUpdateFails()
        {
            var productPriceDto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 180,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = DateTime.UtcNow.AddDays(30)
            };

            _mockService.Setup(s => s.UpdatePriceAsync(It.IsAny<ProductPrice>()))
                        .ReturnsAsync(false);

            var result = await _controller.UpdatePrice(1, productPriceDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePrice_ReturnsNoContent_WhenSuccessful()
        {
            _mockService.Setup(s => s.DeletePriceAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeletePrice(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePrice_ReturnsNotFound_WhenPriceNotFound()
        {
            _mockService.Setup(s => s.DeletePriceAsync(1)).ReturnsAsync(false);

            var result = await _controller.DeletePrice(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
