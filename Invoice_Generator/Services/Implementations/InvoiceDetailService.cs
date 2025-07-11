using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;

namespace Invoice_Generator.Services.Implementations
{
    public class InvoiceDetailService : IInvoiceDetailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddDetailAsync(InvoiceDetail detail)
        {
            // Calculate values if needed (optional)
            detail.SubTotal = detail.Rate * detail.Quantity;
            detail.GrandTotal = detail.SubTotal + detail.Tax;

            await _unitOfWork.InvoiceDetails.AddAsync(detail);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeleteDetailAsync(int id)
        {
            var existing = await _unitOfWork.InvoiceDetails.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.InvoiceDetails.Delete(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<InvoiceDetail>> GetAllDetailsByInvoiceIdAsync(int invoiceId)
        {
            return _unitOfWork.InvoiceDetails
                .Query()
                .Where(d => d.InvoiceId == invoiceId)
                .ToList();
        }

        public async Task<InvoiceDetail?> GetDetailByIdAsync(int id)
        {
            return await _unitOfWork.InvoiceDetails.GetByIdAsync(id);
        }

        public async Task<bool> UpdateDetailAsync(InvoiceDetail detail)
        {
            var existing = await _unitOfWork.InvoiceDetails.GetByIdAsync(detail.Id);
            if (existing == null) return false;

            existing.ProductId = detail.ProductId;
            existing.Quantity = detail.Quantity;
            existing.Rate = detail.Rate;
            existing.SubTotal = detail.Rate * detail.Quantity;
            existing.Tax = detail.Tax;
            existing.GrandTotal = existing.SubTotal + existing.Tax;
            existing.Total = existing.SubTotal + existing.Tax; // optional, if different from GrandTotal

            _unitOfWork.InvoiceDetails.Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
