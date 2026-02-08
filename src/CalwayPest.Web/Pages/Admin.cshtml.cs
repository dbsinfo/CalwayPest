using System;
using System.Threading.Tasks;
using CalwayPest.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Domain.Repositories;

namespace CalwayPest.Web.Pages
{
    public class AdminModel : PageModel
    {
        private readonly IRepository<AdminUser, Guid> _adminUserRepository;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public AdminModel(IRepository<AdminUser, Guid> adminUserRepository)
        {
            _adminUserRepository = adminUserRepository;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Username and password are required.";
                return Page();
            }

            var adminUser = await _adminUserRepository.FirstOrDefaultAsync(
                x => x.Username == username && x.Password == password
            );

            if (adminUser != null)
            {
                // Store admin session and redirect to dashboard
                HttpContext.Session.SetString("AdminUser", username);
                return RedirectToPage("/Dashboard");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }
    }
}
