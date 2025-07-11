using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;

namespace Invoice_Generator.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {

        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddInvoiceAsync(InvoiceCreateDto invoice)
        {
            var invoiceModel = new Invoice
            {
                CustomerId = invoice.CustomerId,
                InvoiceDate = invoice.InvoiceDate,
            };

            await _unitOfWork.Invoices.AddAsync(invoiceModel);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null) return false;

            _unitOfWork.Invoices.Delete(invoice);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _unitOfWork.Invoices.GetAllAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _unitOfWork.Invoices.GetByIdAsync(id);
        }
    }
}

