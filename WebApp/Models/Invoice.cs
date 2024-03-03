namespace WebApp.Models;

public class Invoice
{
    public string InvoiceID { get; set; }
    public string InvoiceDate { get; set; }
    public string ClientName { get; set; }
    public string ClientRC { get; set; }
    public string ClientAddress { get; set; }
    public string ClientPhone { get; set; }
    public string ClientBank { get; set; }
    public string SupplierName { get; set; }
    public string SupplierRC { get; set; }
    public string SupplierAddress { get; set; }
    public string SupplierPhone { get; set; }
    public string SupplierBank { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
