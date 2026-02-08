using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalwayPest.Web.Services;
using Volo.Abp.Domain.Repositories;
using CalwayPest.Domain;

namespace CalwayPest.Web.Pages
{
    public class InvoiceModel : PageModel
    {
        private readonly ICustomEmailSender _emailSender;
        private readonly IRepository<Invoice, Guid> _invoiceRepository;

        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public InvoiceModel(
            ICustomEmailSender emailSender,
            IRepository<Invoice, Guid> invoiceRepository)
        {
            _emailSender = emailSender;
            _invoiceRepository = invoiceRepository;
        }

        public IActionResult OnGet()
        {
            // Check if admin is logged in
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser))
            {
                return RedirectToPage("/Admin");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
            string invoiceNumber,
            string invoiceDate,
            string customerName,
            string customerEmail,
            string customerPhone,
            string customerAddress,
            List<InvoiceItem> items,
            decimal subtotalAmount,
            decimal gstTotalAmount,
            decimal totalAmount,
            string action)
        {
            // Check if admin is logged in
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser))
            {
                return RedirectToPage("/Admin");
            }

            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerEmail))
                {
                    Message = "Customer name and email are required.";
                    IsSuccess = false;
                    return Page();
                }

                if (items == null || !items.Any() || items.All(i => string.IsNullOrWhiteSpace(i.Description)))
                {
                    Message = "At least one invoice item is required.";
                    IsSuccess = false;
                    return Page();
                }

                // Filter out empty items
                var validItems = items.Where(i => !string.IsNullOrWhiteSpace(i.Description)).ToList();

                // Create invoice entity
                var invoice = new Invoice(
                    Guid.NewGuid(),
                    invoiceNumber,
                    DateTime.Parse(invoiceDate),
                    customerName,
                    customerEmail,
                    customerPhone ?? "",
                    customerAddress ?? "",
                    subtotalAmount,
                    gstTotalAmount,
                    totalAmount,
                    action == "send" ? "Sent" : "Draft"
                );

                // Set sent date if sending
                if (action == "send")
                {
                    invoice.SentDate = DateTime.Now;
                }

                // Add invoice items
                foreach (var item in validItems)
                {
                    var invoiceItem = new CalwayPest.Domain.InvoiceItem(
                        invoice.Id,
                        item.Description ?? "",
                        item.Quantity,
                        item.UnitPrice,
                        item.GstType ?? "NoGST"
                    );
                    invoice.Items.Add(invoiceItem);
                }

                // Save invoice to database
                await _invoiceRepository.InsertAsync(invoice);

                // Send email if action is send
                if (action == "send")
                {
                    // Generate invoice HTML for email
                    var invoiceHtml = GenerateInvoiceHtml(
                        invoiceNumber,
                        invoiceDate,
                        customerName,
                        customerEmail,
                        customerPhone,
                        customerAddress,
                        validItems,
                        subtotalAmount,
                        gstTotalAmount,
                        totalAmount
                    );

                    // Send email
                    await _emailSender.SendEmailAsync(
                        customerEmail,
                        $"Invoice {invoiceNumber} from Calway Pest Control",
                        invoiceHtml,
                        isBodyHtml: true
                    );

                    Message = $"Invoice {invoiceNumber} has been successfully generated, saved, and sent to {customerEmail}!";
                }
                else
                {
                    Message = $"Invoice {invoiceNumber} has been saved as draft.";
                }

                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Message = $"Error processing invoice: {ex.Message}";
                IsSuccess = false;
            }

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Remove("AdminUser");
            return RedirectToPage("/Admin");
        }

        private string GenerateInvoiceHtml(
            string invoiceNumber,
            string invoiceDate,
            string customerName,
            string customerEmail,
            string? customerPhone,
            string? customerAddress,
            List<InvoiceItem> items,
            decimal subtotal,
            decimal gst,
            decimal total)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='UTF-8'>");
            sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendLine("<title>Invoice</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body{font-family:Arial,sans-serif;max-width:800px;margin:0 auto;padding:20px;color:#333;}");
            sb.AppendLine(".header{display:flex;justify-content:space-between;margin-bottom:30px;padding-bottom:20px;border-bottom:3px solid #4A6B4F;}");
            sb.AppendLine(".logo{max-width:250px;}");
            sb.AppendLine(".invoice-info{text-align:right;}");
            sb.AppendLine(".invoice-info h2{color:#4A6B4F;margin:0 0 10px 0;}");
            sb.AppendLine(".section{margin-bottom:20px;}");
            sb.AppendLine(".section h3{color:#4A6B4F;margin-bottom:10px;}");
            sb.AppendLine("table{width:100%;border-collapse:collapse;margin:20px 0;}");
            sb.AppendLine("table th,table td{padding:12px;text-align:left;border-bottom:1px solid #ddd;}");
            sb.AppendLine("table th{background-color:#4A6B4F;color:#fff;}");
            sb.AppendLine(".totals{text-align:right;margin-top:20px;}");
            sb.AppendLine(".totals p{margin:5px 0;}");
            sb.AppendLine(".grand-total{font-size:1.3em;color:#4A6B4F;font-weight:bold;margin-top:10px;padding-top:10px;border-top:2px solid #4A6B4F;}");
            sb.AppendLine(".footer{margin-top:40px;padding-top:20px;border-top:1px solid #ddd;text-align:center;color:#666;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Header
            sb.AppendLine("<div class='header'>");
            sb.AppendLine("<div>");
            sb.AppendLine("<h1 style='color:#4A6B4F;margin:0;'>Calway Pest Control</h1>");
            sb.AppendLine("<p>Calgary, AB<br>Email: info@calwaypest.ca</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='invoice-info'>");
            sb.AppendLine("<h2>INVOICE</h2>");
            sb.AppendLine($"<p><strong>Invoice #:</strong> {invoiceNumber}</p>");
            sb.AppendLine($"<p><strong>Date:</strong> {DateTime.Parse(invoiceDate):MMMM dd, yyyy}</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            // Customer Details
            sb.AppendLine("<div class='section'>");
            sb.AppendLine("<h3>Bill To:</h3>");
            sb.AppendLine($"<p><strong>{customerName}</strong><br>");
            sb.AppendLine($"{customerEmail}<br>");
            if (!string.IsNullOrWhiteSpace(customerPhone))
                sb.AppendLine($"{customerPhone}<br>");
            if (!string.IsNullOrWhiteSpace(customerAddress))
                sb.AppendLine($"{customerAddress}");
            sb.AppendLine("</p>");
            sb.AppendLine("</div>");

            // Items Table
            sb.AppendLine("<table>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th style='width:5%;'>#</th>");
            sb.AppendLine("<th style='width:40%;'>Description</th>");
            sb.AppendLine("<th style='width:10%;'>Qty</th>");
            sb.AppendLine("<th style='width:15%;'>Unit Price</th>");
            sb.AppendLine("<th style='width:10%;'>GST</th>");
            sb.AppendLine("<th style='width:20%;text-align:right;'>Amount</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            int itemNumber = 1;
            foreach (var item in items)
            {
                var amount = item.Quantity * item.UnitPrice;
                var gstLabel = item.GstType == "included" ? "Inc." : item.GstType == "excluded" ? "Exc." : "N/A";

                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{itemNumber++}</td>");
                sb.AppendLine($"<td>{item.Description}</td>");
                sb.AppendLine($"<td>{item.Quantity}</td>");
                sb.AppendLine($"<td>${item.UnitPrice:F2}</td>");
                sb.AppendLine($"<td>{gstLabel}</td>");
                sb.AppendLine($"<td style='text-align:right;'>${amount:F2}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            // Totals
            sb.AppendLine("<div class='totals'>");
            sb.AppendLine($"<p><strong>Subtotal:</strong> ${subtotal:F2}</p>");
            sb.AppendLine($"<p><strong>GST (5%):</strong> ${gst:F2}</p>");
            sb.AppendLine($"<p class='grand-total'>Total: ${total:F2}</p>");
            sb.AppendLine("</div>");

            // Footer
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine("<p>Thank you for your business!</p>");
            sb.AppendLine("<p>If you have any questions about this invoice, please contact us.</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }

    public class InvoiceItem
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string GstType { get; set; } = string.Empty;
    }
}
