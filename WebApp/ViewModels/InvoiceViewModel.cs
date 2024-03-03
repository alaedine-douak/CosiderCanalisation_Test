using WebApp.Models;

namespace WebApp.ViewModels;

public class InvoiceViewModel
{
    public string InvoiceID { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public string ClientName { get; set; }
    public string SupplierName { get; set; }
    public double TaxAmount { get; set; }
    public double ItemsPriceBeforeTax { get; set; }
    public double ItemsPriceAfterTax { get; set; }

    public IEnumerable<InvoiceItem>  InvoiceItems { get; set; } = Enumerable.Empty<InvoiceItem>();
}
