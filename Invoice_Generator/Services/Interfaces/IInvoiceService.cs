using Invoice_Generator.DTOs;
using Invoice_Generator.Models;

namespace Invoice_Generator.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task AddInvoiceAsync(InvoiceCreateDto invoice);
        Task<bool> DeleteInvoiceAsync(int id);
    }
}
