using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using CalwayPest.Domain;

namespace CalwayPest.Web.Pages
{
    public class InvoiceDetailModel : PageModel
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;

        public InvoiceDetailModel(IRepository<Invoice, Guid> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public Invoice? Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            // Check if admin is logged in
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser))
            {
                return RedirectToPage("/Admin");
            }

            // Get invoice with items
            var query = await _invoiceRepository.GetQueryableAsync();
            Invoice = await query
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

            return Page();
        }

        public async Task<IActionResult> OnPostMarkPaidAsync(Guid invoiceId)
        {
            // Check if admin is logged in
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser))
            {
                return RedirectToPage("/Admin");
            }

            var invoice = await _invoiceRepository.GetAsync(invoiceId);
            if (invoice != null && invoice.Status == "Sent")
            {
                invoice.Status = "Paid";
                invoice.PaidDate = DateTime.Now;
                await _invoiceRepository.UpdateAsync(invoice);
            }

            return RedirectToPage("/InvoiceDetail", new { id = invoiceId });
        }

        public async Task<IActionResult> OnPostSendInvoiceAsync(Guid invoiceId)
        {
            // Check if admin is logged in
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser))
            {
                return RedirectToPage("/Admin");
            }

            var invoice = await _invoiceRepository.GetAsync(invoiceId);
            if (invoice != null && invoice.Status == "Draft")
            {
                invoice.Status = "Sent";
                invoice.SentDate = DateTime.Now;
                await _invoiceRepository.UpdateAsync(invoice);
                
                // TODO: Send email to customer here if needed
            }

            return RedirectToPage("/InvoiceDetail", new { id = invoiceId });
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Remove("AdminUser");
            return RedirectToPage("/Admin");
        }
    }
}
