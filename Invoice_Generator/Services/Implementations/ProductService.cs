using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;

namespace Invoice_Generator.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Product>> GetAllProductAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(product.Id);
            if (existing == null) return false;

            existing.Name = product.Name;
            existing.TaxPercentage = product.TaxPercentage;
            existing.CategoryId = product.CategoryId;
            existing.Description = product.Description;

            _unitOfWork.Products.Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.Products.Delete(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public Task<decimal?> GetPriceForTodayAsync(int productId)
        {
            throw new NotImplementedException();
        }
    }
}
