using WebApp.Models;
using WebApp.Utility;
using WebApp.Services;
using QuestPDF.Fluent;
using System.Text.Json;
using WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class InvoiceController : Controller
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var invoices = await _invoiceService.GetAllInvoices();

        var invoiceModel = invoices.Select(x => new InvoiceViewModel
        {
            InvoiceID = x.InvoiceID,
            InvoiceDate = DateOnly.ParseExact(x.InvoiceDate, "yyyy-MM-dd"),
            ClientName = x.ClientName,
            SupplierName = x.SupplierName,
            ItemsPriceAfterTax = x.InvoiceItems.Select(i => (i.ItemPrice * i.ItemQuantity) + (i.ItemTax * i.ItemQuantity)).Sum()
        })
        .ToList();

        return View(invoiceModel);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string invoiceId)
    {
        var invoices = await _invoiceService.GetAllInvoices();

        var selectedInvoice = invoices
            .Where(x => x.InvoiceID == invoiceId)
            .FirstOrDefault();

        if (selectedInvoice != null)
        {
            var invoiceViewModel = new InvoiceViewModel
            {
                InvoiceID = selectedInvoice.InvoiceID,
                InvoiceDate = DateOnly.ParseExact(selectedInvoice.InvoiceDate, "yyyy-MM-dd"),
                ClientName = selectedInvoice.ClientName,
                SupplierName = selectedInvoice.SupplierName,
                ItemsPriceBeforeTax = selectedInvoice.InvoiceItems.Select(x => x.ItemPrice * x.ItemQuantity).Sum(),
                ItemsPriceAfterTax = selectedInvoice.InvoiceItems.Select(x => (x.ItemPrice * x.ItemQuantity) + (x.ItemTax * x.ItemQuantity)).Sum(),
                TaxAmount = selectedInvoice.InvoiceItems.Select(x => x.ItemTax * x.ItemQuantity).Sum(),
                InvoiceItems = selectedInvoice.InvoiceItems
            };

            return View(invoiceViewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GeneratePdfInvoice(string invoiceId)
    {
        var invoices = await _invoiceService.GetAllInvoices();


        var invoiceModel = invoices.Select(invoice =>
            new Invoice
            {
                InvoiceID = invoice.InvoiceID,
                InvoiceDate = invoice.InvoiceDate.FormatDateToFrenchDate("dd MMM yyyy"),
                SupplierName = invoice.SupplierName,
                ClientName = invoice.ClientName,
                InvoiceItems = invoice.InvoiceItems.Select(item => new InvoiceItem
                {
                    ItemID = item.ItemID,
                    ItemLibelle = item.ItemLibelle,
                    ItemQuantity = item.ItemQuantity,
                    ItemPrice = item.ItemPrice,
                    ItemTax = item.ItemTax
                }).ToList()
            })
            .Where(x => x.InvoiceID == invoiceId)
            .First();
            

        var pdf = new PdfInvoiceDocument(invoiceModel);
        var pdfStream = pdf.GeneratePdf();

        MemoryStream ms = new MemoryStream(pdfStream);

        return new FileStreamResult(ms, "application/pdf");
    }


    [HttpPost]
    [Route("/search")]
    public async Task<IActionResult> SearchByInvoiceItem()
    {
        var requestBody = await Request.Body.ReadAsStringAsync();
        var request = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

        var itemLibelle = request?["itemLibelle"];

        var invoices = await _invoiceService.GetAllInvoices();

        if (string.IsNullOrWhiteSpace(itemLibelle))
        {
            // try to redirect to index
            var invoicesModel = invoices.Select(invoice => new InvoiceViewModel
            {
                InvoiceID = invoice.InvoiceID,
                InvoiceDate = DateOnly.ParseExact(invoice.InvoiceDate, "yyyy-MM-dd"),
                ClientName = invoice.ClientName,
                SupplierName = invoice.SupplierName,
                ItemsPriceAfterTax = invoice.InvoiceItems.Select(x => (x.ItemPrice * x.ItemQuantity) + (x.ItemTax * x.ItemQuantity)).Sum(),
            })
            .ToList();

            return Json(invoicesModel);
        }


        var items = invoices.SelectMany(item => item.InvoiceItems)
            .Where(x => x.ItemLibelle.ToLower().Contains(itemLibelle));

        return Json(items);
    }
}
