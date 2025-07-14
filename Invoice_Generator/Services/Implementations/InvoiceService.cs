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

        public async Task AddInvoiceAsync(InvoiceRequestDto invoice)
        {
            var invoiceModel = new Invoice
            {
                CustomerId = invoice.CustomerId,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceDetails = new List<InvoiceDetail>()
            };

            decimal grandTotal = 0;
            var today = DateTime.UtcNow.Date;

            foreach (var item in invoice.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {item.ProductId} does not exist.");
                }

                var productPrice = _unitOfWork.ProductPrices.Query()
                    .Where(p => p.ProductId == item.ProductId &&
                                today >= p.EffectiveFrom.Date &&
                                (p.EffectiveTo == null || today <= p.EffectiveTo.Value.Date))
                    .FirstOrDefault()?.Price ?? 0;

                var qty = item.Quantity;
                var subTotal = productPrice * qty;
                var taxAmt = subTotal * (product.TaxPercentage)/100;
                var totalAmt = subTotal + taxAmt;

                invoiceModel.InvoiceDetails.Add(new InvoiceDetail
                {
                    ProductId = item.ProductId,
                    Quantity = qty,
                    Rate = productPrice,
                    SubTotal = subTotal,
                    Tax = taxAmt,
                    Total = totalAmt,
                    GrandTotal = totalAmt
                });

                invoiceModel.SubTotal = subTotal;
                invoiceModel.TaxTotal = taxAmt;

                grandTotal += totalAmt;
            }

            invoiceModel.GrandTotal = grandTotal;

            await _unitOfWork.Invoices.AddAsync(invoiceModel);

            foreach (var detail in invoiceModel.InvoiceDetails)
            {
                await _unitOfWork.InvoiceDetails.AddAsync(detail);
            }

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

