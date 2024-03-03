namespace WebApp.Models;

public class InvoiceItem
{
    public string ItemID { get; set; }
    public string ItemLibelle { get; set; }
    public string ItemUnit { get; set; }
    public int ItemQuantity { get; set; }
    public double ItemPrice { get; set; }
    public double ItemTax { get; set; }
}
