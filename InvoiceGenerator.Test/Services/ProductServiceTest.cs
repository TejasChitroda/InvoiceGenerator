using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoice_Generator.Models;
using Invoice_Generator.Repository;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using Xunit;

public class ProductServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Product>> _productRepoMock;
    private readonly ProductService _service;

    public ProductServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepoMock = new Mock<IGenericRepository<Product>>();
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _service = new ProductService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllProductAsync_ReturnsAllProducts()
    {
        var products = new List<Product> { new Product { Id = 1 }, new Product { Id = 2 } };
        _productRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _service.GetAllProductAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetProductByIdAsync_ProductExists_ReturnsProduct()
    {
        var product = new Product { Id = 1 };
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.GetProductByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_ProductDoesNotExist_ReturnsNull()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var result = await _service.GetProductByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddProductAsync_AddsProductAndSaves()
    {
        var product = new Product { Id = 1 };
        _productRepoMock.Setup(r => r.AddAsync(product)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _service.AddProductAsync(product);

        _productRepoMock.Verify(r => r.AddAsync(product), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ProductExists_UpdatesAndSaves()
    {
        var product = new Product { Id = 1, Name = "New", TaxPercentage = 5, CategoryId = 2, Description = "desc" };
        var existing = new Product { Id = 1 };
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.UpdateAsync(product);

        Assert.True(result);
        _productRepoMock.Verify(r => r.Update(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        Assert.Equal("New", existing.Name);
        Assert.Equal(5, existing.TaxPercentage);
        Assert.Equal(2, existing.CategoryId);
        Assert.Equal("desc", existing.Description);
    }

    [Fact]
    public async Task UpdateAsync_ProductDoesNotExist_ReturnsFalse()
    {
        var product = new Product { Id = 1 };
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var result = await _service.UpdateAsync(product);

        Assert.False(result);
        _productRepoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ProductExists_DeletesAndSaves()
    {
        var existing = new Product { Id = 1 };
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _productRepoMock.Verify(r => r.Delete(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ProductDoesNotExist_ReturnsFalse()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);
        _productRepoMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task GetPriceForTodayAsync_ThrowsNotImplementedException()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() => _service.GetPriceForTodayAsync(1));
    }
}
