using WebApp.Models;

namespace WebApp.Services;

public interface IInvoiceService
{
    Task<IEnumerable<Invoice>> GetAllInvoices();
}
