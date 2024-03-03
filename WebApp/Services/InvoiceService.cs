using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using WebApp.Models;

namespace WebApp.Services;

public class InvoiceService : IInvoiceService
{
    private readonly HttpClient _httpClient;
    private readonly InvoiceUrlSettings _invoiceUrl;

   public InvoiceService(IHttpClientFactory httpClientFactory, IOptions<InvoiceUrlSettings> options)
    {
        _httpClient = httpClientFactory.CreateClient();
        _invoiceUrl = options.Value;
    }

    public async Task<IEnumerable<Invoice>> GetAllInvoices()
    {
        var httRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            _invoiceUrl.Url)
        {
            Headers =
            {
                { HeaderNames.Accept, "application/json" }
            }
        };

        var httpResponse = await _httpClient.SendAsync(httRequestMessage);

        using var contentStream = await httpResponse.Content.ReadAsStreamAsync();

        var invoices = await JsonSerializer.DeserializeAsync<IEnumerable<Invoice>>(contentStream);

        return invoices;
    }
}
