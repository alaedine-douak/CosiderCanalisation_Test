using WebApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebApp.Utility.PdfComponents;


namespace WebApp.Utility;

public class PdfInvoiceDocument : IPdfInvoiceDocument
{
    public Invoice Invoice { get; }

    public PdfInvoiceDocument(Invoice invoice)
    {
        Invoice = invoice;
    }

    public DocumentMetadata GetMetadata() 
        => DocumentMetadata.Default;

    public DocumentSettings GetSettings() 
        => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(3, Unit.Centimetre);
                page.MarginHorizontal(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(20));

                page.Header().Row(row =>
                {
                    var titleStyle = TextStyle.Default.FontSize(16).ExtraBold().Italic();

                    row.RelativeItem().Column(column =>
                    {
                        column.Item().AlignCenter().PaddingBottom(5).Text($"Facture  N° {Invoice.InvoiceID}").Style(titleStyle);

                        column.Item().AlignRight().PaddingRight(70).Text(text =>
                        {
                            text.Span("Date de facture:  ").FontSize(11);
                            //text.Span("23 Jan 2024").FontSize(11);
                            text.Span(Invoice.InvoiceDate).FontSize(11);
                        });
                    });
                });


                page.Content().PaddingVertical(40).Column(column =>
                {
                    column.Spacing(5);

                    column.Item().Row(row =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Component(
                                new ParticipantPdfComponent("FOURNISSEUR", $"{Invoice.SupplierName}"));
                            row.ConstantItem(50);
                            row.RelativeItem().Component(
                                new ParticipantPdfComponent("CLIENT", $"{Invoice.ClientName}"));
                        });
                    });


                    column.Item().PaddingTop(20).PaddingBottom(10).Element(ComposeTable);


                    column.Item().AlignRight().Width(198).Table(table =>
                    {
                        table.ColumnsDefinition(column =>
                        {
                            // column.RelativeColumn();
                            column.RelativeColumn(1.4f);
                            // column.ConstantColumn(100);
                            column.RelativeColumn();
                        });


                        var totalPrice = Invoice.InvoiceItems.Select(x =>
                            (x.ItemPrice * x.ItemQuantity)).Sum();

                        table.Cell().Element(CellStyle).AlignLeft().PaddingHorizontal(5).AlignMiddle().Text("TOTAL");
                        table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(string.Format("{0:0.00}", totalPrice));

                        var tvaAmount = Invoice.InvoiceItems.Select(x => 
                            (x.ItemTax * x.ItemQuantity)).Sum();

                        table.Cell().Element(CellStyle).AlignLeft().PaddingHorizontal(5).AlignMiddle().Text("TVA");
                        table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(string.Format("{0:0.00}", tvaAmount));

                        var ttcTotal = Invoice.InvoiceItems.Select(x =>
                            (x.ItemPrice * x.ItemQuantity) + (x.ItemTax * x.ItemQuantity)).Sum();

                        table.Cell().Element(CellStyle).DefaultTextStyle(x => x.Bold())
                            .AlignLeft().PaddingHorizontal(5).AlignMiddle().Text("Total TTC").Italic();
                        table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(string.Format("{0:0.00}", ttcTotal));


                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .DefaultTextStyle(x => x.FontSize(10))
                                .MinHeight(24)
                                .Border(0.5f);
                        }
                    });


                    column.Item()
                        .AlignRight()
                        .PaddingHorizontal(100)
                        .PaddingTop(100)
                        .DefaultTextStyle(x => x.FontSize(12)).Text("LA SIGNATURE").Italic();

                });

            });
    }

    void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(column =>
            {
                column.ConstantColumn(30);
                column.RelativeColumn(6);
                column.RelativeColumn(3);
                column.RelativeColumn(2);
                column.RelativeColumn(2);
                column.RelativeColumn(3);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text("N°");
                header.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text("LIBELLE");
                header.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text("QUANTITÉ");
                header.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text("PRIX");
                header.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text("HT");
                header.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text("TTC");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.FontSize(10))
                        // .PaddingVertical(20)
                        .MinHeight(25)
                        // .AlignMiddle()
                        // .MinWidth(50)
                        .Border(0.5f)
                        .Background("#fabf8f");
                    // .BorderColor(Colors.Black);
                }
            });

            foreach (var item in Invoice.InvoiceItems)
            {
                var itemIdx = Invoice.InvoiceItems.ToList().IndexOf(item) + 1;

                table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(itemIdx.ToString());
                table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(item.ItemLibelle);
                table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(item.ItemQuantity.ToString());
                table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(string.Format("{0:0.00}", item.ItemPrice));
                table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(string.Format("{0:0.00}", (item.ItemPrice * item.ItemQuantity)));
                table.Cell().Element(CellStyle).AlignCenter().AlignMiddle().Text(string.Format("{0:0.00}", (item.ItemPrice * item.ItemQuantity) + (item.ItemTax * item.ItemQuantity)));

                static IContainer CellStyle(IContainer container)
                {
                    return container
                        .DefaultTextStyle(x => x.FontSize(10))
                        .MinHeight(25)
                        .Border(0.5f);
                }
            }
        });
    }


}
