using Invoice_Generator.Models;

namespace Invoice_Generator.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<decimal?> GetPriceForTodayAsync(int productId);
        Task AddProductAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
