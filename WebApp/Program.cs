using WebApp;
using WebApp.Services;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<InvoiceUrlSettings>(
    builder.Configuration.GetSection("InvoiceUrl"));


builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddHttpClient<InvoiceService>();

builder.Services.AddControllersWithViews();

QuestPDF.Settings.License = LicenseType.Community;


var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.MapControllerRoute("invoice",
    pattern: "{Controller=Invoice}/{Action=Index}");

app.Run();