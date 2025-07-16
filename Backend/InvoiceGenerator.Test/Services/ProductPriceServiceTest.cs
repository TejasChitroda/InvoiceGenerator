using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoice_Generator.Models;
using Invoice_Generator.Repository;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using Xunit;

public class ProductPriceServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<ProductPrice>> _productPriceRepoMock;
    private readonly IProductPriceService _service;

    public ProductPriceServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productPriceRepoMock = new Mock<IGenericRepository<ProductPrice>>();
        _unitOfWorkMock.Setup(u => u.ProductPrices).Returns(_productPriceRepoMock.Object);
        _service = new IProductPriceService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddPriceAsync_AddsPriceAndSaves()
    {
        var price = new ProductPrice { Id = 1, ProductId = 2, Price = 10, EffectiveFrom = DateTime.UtcNow, EffectiveTo = DateTime.UtcNow.AddDays(1) };
        _productPriceRepoMock.Setup(r => r.AddAsync(price)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _service.AddPriceAsync(price);

        _productPriceRepoMock.Verify(r => r.AddAsync(price), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePriceAsync_PriceExists_DeletesAndReturnsTrue()
    {
        var price = new ProductPrice { Id = 1 };
        _productPriceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(price);

        var result = await _service.DeletePriceAsync(1);

        Assert.True(result);
        _productPriceRepoMock.Verify(r => r.Delete(price), Times.Once);
    }

    [Fact]
    public async Task DeletePriceAsync_PriceDoesNotExist_ReturnsFalse()
    {
        _productPriceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductPrice?)null);

        var result = await _service.DeletePriceAsync(1);

        Assert.False(result);
        _productPriceRepoMock.Verify(r => r.Delete(It.IsAny<ProductPrice>()), Times.Never);
    }

    [Fact]
    public async Task GetAllPriceAsync_ReturnsAllPrices()
    {
        var prices = new List<ProductPrice> { new ProductPrice { Id = 1 }, new ProductPrice { Id = 2 } };
        _productPriceRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(prices);

        var result = await _service.GetAllPriceAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPriceByIdAsync_PriceExists_ReturnsPrice()
    {
        var price = new ProductPrice { Id = 1 };
        _productPriceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(price);

        var result = await _service.GetPriceByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetPriceByIdAsync_PriceDoesNotExist_ReturnsNull()
    {
        _productPriceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductPrice?)null);

        var result = await _service.GetPriceByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPriceByProductIdAsync_ReturnsCorrectPrice()
    {
        var today = DateTime.UtcNow;
        var prices = new List<ProductPrice>
        {
            new ProductPrice { Id = 1, ProductId = 2, EffectiveFrom = today.AddDays(-2), EffectiveTo = today.AddDays(2), Price = 100 },
            new ProductPrice { Id = 2, ProductId = 2, EffectiveFrom = today.AddDays(-1), EffectiveTo = today.AddDays(1), Price = 200 }
        }.AsQueryable();

        _productPriceRepoMock.Setup(r => r.Query()).Returns(prices);

        var result = await _service.GetPriceByProductIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal(2, result.Id); // Should return the most recent EffectiveFrom
    }

    [Fact]
    public async Task UpdatePriceAsync_PriceExists_UpdatesAndSaves()
    {
        var price = new ProductPrice
        {
            Id = 1,
            ProductId = 2,
            Price = 50,
            EffectiveFrom = DateTime.UtcNow,
            EffectiveTo = DateTime.UtcNow.AddDays(1)
        };
        var existing = new ProductPrice { Id = 1 };
        _productPriceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.UpdatePriceAsync(price);

        Assert.True(result);
        _productPriceRepoMock.Verify(r => r.Update(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        Assert.Equal(price.ProductId, existing.ProductId);
        Assert.Equal(price.Price, existing.Price);
        Assert.Equal(price.EffectiveFrom, existing.EffectiveFrom);
        Assert.Equal(price.EffectiveTo, existing.EffectiveTo);
    }
}
