using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Domain.Repositories;
using CalwayPest.Domain;

namespace CalwayPest.Web.Pages
{
    public class InvoiceListModel : PageModel
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;

        public InvoiceListModel(IRepository<Invoice, Guid> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        
        [BindProperty(SupportsGet = true)]
        public string? InvoiceNumberFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? CustomerNameFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDateFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? ToDateFilter { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if admin is logged in
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser))
            {
                return RedirectToPage("/Admin");
            }

            // Get all invoices
            var query = await _invoiceRepository.GetQueryableAsync();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(InvoiceNumberFilter))
            {
                query = query.Where(i => i.InvoiceNumber.Contains(InvoiceNumberFilter));
            }

            if (!string.IsNullOrWhiteSpace(CustomerNameFilter))
            {
                query = query.Where(i => i.CustomerName.Contains(CustomerNameFilter));
            }

            if (!string.IsNullOrWhiteSpace(StatusFilter))
            {
                query = query.Where(i => i.Status == StatusFilter);
            }

            if (FromDateFilter.HasValue)
            {
                query = query.Where(i => i.InvoiceDate >= FromDateFilter.Value);
            }

            if (ToDateFilter.HasValue)
            {
                query = query.Where(i => i.InvoiceDate <= ToDateFilter.Value);
            }

            // Order by invoice date descending (newest first)
            Invoices = query.OrderByDescending(i => i.InvoiceDate).ToList();

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Remove("AdminUser");
            return RedirectToPage("/Admin");
        }
    }
}
