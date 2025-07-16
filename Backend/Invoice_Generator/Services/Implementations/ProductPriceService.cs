using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;

namespace Invoice_Generator.Services.Implementations
{
    public class IProductPriceService : IProductPrice
    {
        private readonly IUnitOfWork _unitOfWork;

        public IProductPriceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddPriceAsync(ProductPrice productPrice)
        {
            await _unitOfWork.ProductPrices.AddAsync(productPrice);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeletePriceAsync(int id)
        {
            var price = await _unitOfWork.ProductPrices.GetByIdAsync(id);
            if(price == null)
            {
                return false;
            }
            _unitOfWork.ProductPrices.Delete(price);
            return true;
        }

        public Task<IEnumerable<ProductPrice>> GetAllPriceAsync()
        {
            return _unitOfWork.ProductPrices.GetAllAsync();
        }

        public Task<ProductPrice?> GetPriceByIdAsync(int id)
        {
            return _unitOfWork.ProductPrices.GetByIdAsync(id);
        }

        public Task<ProductPrice> GetPriceByProductIdAsync(int productId)
        {

            var today = DateTime.UtcNow;

            var price =  _unitOfWork.ProductPrices
                .Query()
                .Where(p => p.ProductId == productId &&
                            p.EffectiveFrom <= today &&
                            p.EffectiveTo >= today)
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefault();

            return Task.FromResult(price);
        }

        public async Task<bool> UpdatePriceAsync(ProductPrice productPrice)
        {
            var existing = await _unitOfWork.ProductPrices.GetByIdAsync(productPrice.Id);

            existing.EffectiveFrom = productPrice.EffectiveFrom;
            existing.EffectiveTo = productPrice.EffectiveTo;
            existing.Price = productPrice.Price;
            existing.ProductId = productPrice.ProductId;

            _unitOfWork.ProductPrices.Update(existing);
            await _unitOfWork.SaveAsync();
            return true;

        }
    }
}
