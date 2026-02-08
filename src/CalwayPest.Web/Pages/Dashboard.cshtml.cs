using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Domain.Repositories;
using CalwayPest.Domain;

namespace CalwayPest.Web.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;

        public DashboardModel(IRepository<Invoice, Guid> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public decimal TotalRevenue { get; set; }
        public int PaidInvoiceCount { get; set; }
        public int PendingInvoiceCount { get; set; }
        public int TotalInvoiceCount { get; set; }
        public string SalesChartDataJson { get; set; } = "[]";

        public async Task<IActionResult> OnGetAsync(int days = 30)
        {
            // Check if admin is logged in
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser))
            {
                return RedirectToPage("/Admin");
            }

            // Get all invoices
            var allInvoices = await _invoiceRepository.GetListAsync();

            // Calculate statistics
            TotalInvoiceCount = allInvoices.Count;
            PaidInvoiceCount = allInvoices.Count(i => i.Status == "Paid");
            PendingInvoiceCount = allInvoices.Count(i => i.Status == "Sent" || i.Status == "Draft");
            // Revenue includes both Paid and Sent invoices
            TotalRevenue = allInvoices.Where(i => i.Status == "Paid" || i.Status == "Sent").Sum(i => i.TotalAmount);

            // Get sales data for the chart (for the specified number of days)
            // Include both Paid and Sent invoices in sales chart
            var startDate = DateTime.Now.Date.AddDays(-days);
            var salesByDay = allInvoices
                .Where(i => i.InvoiceDate >= startDate && (i.Status == "Paid" || i.Status == "Sent"))
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("MMM dd"),
                    amount = g.Sum(i => i.TotalAmount)
                })
                .OrderBy(x => x.date)
                .ToList();

            // Fill in missing days with zero sales
            var chartData = new List<object>();
            for (int i = days - 1; i >= 0; i--)
            {
                var date = DateTime.Now.Date.AddDays(-i);
                var dateStr = date.ToString("MMM dd");
                var dayData = salesByDay.FirstOrDefault(s => s.date == dateStr);
                
                chartData.Add(new
                {
                    date = dateStr,
                    amount = dayData?.amount ?? 0
                });
            }

            SalesChartDataJson = JsonSerializer.Serialize(chartData);

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Remove("AdminUser");
            return RedirectToPage("/Admin");
        }
    }
}
