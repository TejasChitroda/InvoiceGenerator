using Invoice_Generator.Models;

namespace Invoice_Generator.Services.Interfaces
{
    public interface IInvoiceDetailService
    {
        Task<IEnumerable<InvoiceDetail>> GetAllDetailsByInvoiceIdAsync(int invoiceId);
        Task<InvoiceDetail?> GetDetailByIdAsync(int id);
        Task AddDetailAsync(InvoiceDetail detail);
        Task<bool> UpdateDetailAsync(InvoiceDetail detail);
        Task<bool> DeleteDetailAsync(int id);
    }
}
