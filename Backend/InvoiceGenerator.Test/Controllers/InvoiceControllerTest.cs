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
    public class InvoiceControllerTest
    {
        private readonly Mock<IInvoiceService> _mockService;
        private readonly InvoiceController _controller;

        public InvoiceControllerTest()
        {
            _mockService = new Mock<IInvoiceService>();
            _controller = new InvoiceController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllInvoices_ReturnsOkResult_WithListOfInvoices()
        {
            // Arrange
            var invoices = new List<Invoice> { new Invoice { Id = 1, CustomerId = 1 } };
            _mockService.Setup(s => s.GetAllInvoicesAsync()).ReturnsAsync(invoices);

            // Act
            var result = await _controller.GetAllInvoices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(invoices, okResult.Value);
        }

        [Fact]
        public async Task GetInvoiceById_ReturnsOkResult_WhenInvoiceExists()
        {
            // Arrange
            var invoice = new Invoice { Id = 1, CustomerId = 1 };
            _mockService.Setup(s => s.GetInvoiceByIdAsync(1)).ReturnsAsync(invoice);

            // Act
            var result = await _controller.GetInvoiceById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(invoice, okResult.Value);
        }

        [Fact]
        public async Task GetInvoiceById_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetInvoiceByIdAsync(1)).ReturnsAsync((Invoice)null);

            // Act
            var result = await _controller.GetInvoiceById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateInvoice_ReturnsOkResult_WhenModelStateIsValid()
        {
            // Arrange
            var dto = new InvoiceRequestDto { CustomerId = 1, Items = new List<InvoiceItemDto>() };

            // Act
            var result = await _controller.CreateInvoice(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
            _mockService.Verify(s => s.AddInvoiceAsync(dto), Times.Once);
        }

        [Fact]
        public async Task CreateInvoice_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var dto = new InvoiceRequestDto { CustomerId = 1, Items = new List<InvoiceItemDto>() };
            _controller.ModelState.AddModelError("CustomerId", "Required");

            // Act
            var result = await _controller.CreateInvoice(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequest.Value);
        }

        [Fact]
        public async Task DeleteInvoice_ReturnsNoContent_WhenDeleteSucceeds()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteInvoiceAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteInvoice(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteInvoice_ReturnsNotFound_WhenDeleteFails()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteInvoiceAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
