using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Repository;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using Xunit;

public class InvoiceServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Invoice>> _invoiceRepoMock;
    private readonly Mock<IGenericRepository<InvoiceDetail>> _invoiceDetailRepoMock;
    private readonly Mock<IGenericRepository<Customer>> _customerRepoMock;
    private readonly Mock<IGenericRepository<Product>> _productRepoMock;
    private readonly Mock<IGenericRepository<ProductPrice>> _productPriceRepoMock;
    private readonly Mock<IGenericRepository<Category>> _categoryRepoMock;
    private readonly InvoiceService _service;

    public InvoiceServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _invoiceRepoMock = new Mock<IGenericRepository<Invoice>>();
        _invoiceDetailRepoMock = new Mock<IGenericRepository<InvoiceDetail>>();
        _customerRepoMock = new Mock<IGenericRepository<Customer>>();
        _productRepoMock = new Mock<IGenericRepository<Product>>();
        _productPriceRepoMock = new Mock<IGenericRepository<ProductPrice>>();
        _categoryRepoMock = new Mock<IGenericRepository<Category>>();

        _unitOfWorkMock.Setup(u => u.Invoices).Returns(_invoiceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.InvoiceDetails).Returns(_invoiceDetailRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ProductPrices).Returns(_productPriceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);

        _service = new InvoiceService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddInvoiceAsync_AddsInvoiceAndSaves()
    {
        var invoiceRequestDto = new InvoiceRequestDto 
        {
            CustomerId = 1,
            InvoiceDate = DateTime.UtcNow,
            
        };

        _invoiceRepoMock.Setup(r => r.AddAsync(It.IsAny<Invoice>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _service.AddInvoiceAsync(invoiceRequestDto);

        _invoiceRepoMock.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllInvoicesAsync_ReturnsAllInvoices()
    {
        var invoices = new List<Invoice> { new Invoice { Id = 1 }, new Invoice { Id = 2 } };
        _invoiceRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(invoices);

        var result = await _service.GetAllInvoicesAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_InvoiceExists_ReturnsInvoice()
    {
        var invoice = new Invoice { Id = 1 };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(invoice);

        var result = await _service.GetInvoiceByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_InvoiceDoesNotExist_ReturnsNull()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Invoice?)null);

        var result = await _service.GetInvoiceByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_InvoiceExists_DeletesAndSaves()
    {
        var existing = new Invoice { Id = 1 };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.DeleteInvoiceAsync(1);

        Assert.True(result);
        _invoiceRepoMock.Verify(r => r.Delete(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_InvoiceDoesNotExist_ReturnsFalse()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Invoice?)null);

        var result = await _service.DeleteInvoiceAsync(1);

        Assert.False(result);
        _invoiceRepoMock.Verify(r => r.Delete(It.IsAny<Invoice>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }
}
