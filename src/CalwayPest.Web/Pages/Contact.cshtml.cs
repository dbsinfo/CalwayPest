using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CalwayPest.Domain;
using CalwayPest.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace CalwayPest.Web.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IRepository<ContactSubmission, Guid> _contactRepository;
        private readonly ICustomEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactModel> _logger;

        public ContactModel(
            IRepository<ContactSubmission, Guid> contactRepository,
            ICustomEmailSender emailSender,
            IConfiguration configuration,
            ILogger<ContactModel> logger)
        {
            _contactRepository = contactRepository;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost([FromBody] ContactFormData formData)
        {
            try
            {
                _logger.LogInformation("Contact form submission received");
                
                if (formData == null)
                {
                    _logger.LogWarning("Form data is null");
                    return new JsonResult(new { success = false, message = "No data received" });
                }
                
                _logger.LogInformation("Form data: {FullName}, {Email}, Source: {FormSource}", formData.FullName, formData.Email, formData.FormSource);
                
                // Validate based on form source
                if (formData.FormSource == "FooterForm")
                {
                    // Footer form only requires email
                    if (string.IsNullOrWhiteSpace(formData.Email))
                    {
                        return new JsonResult(new { success = false, message = "Email address is required" });
                    }
                }
                else
                {
                    // Main contact form requires all fields
                    if (string.IsNullOrWhiteSpace(formData.FullName) ||
                        string.IsNullOrWhiteSpace(formData.Email) ||
                        string.IsNullOrWhiteSpace(formData.Message))
                    {
                        return new JsonResult(new { success = false, message = "All fields are required" });
                    }
                }

                if (!new EmailAddressAttribute().IsValid(formData.Email))
                {
                    return new JsonResult(new { success = false, message = "Invalid email address" });
                }

                // Save to database
                _logger.LogInformation("Saving contact submission to database from source: {FormSource}", formData.FormSource);
                var submission = new ContactSubmission(
                    Guid.NewGuid(),
                    formData.FullName ?? "Quick Request",
                    formData.Email,
                    formData.Message ?? $"Quick service request from {formData.FormSource}",
                    formData.FormSource
                );

                try
                {
                    await _contactRepository.InsertAsync(submission, autoSave: true);
                    _logger.LogInformation("Contact submission saved successfully with ID: {SubmissionId}", submission.Id);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Failed to save contact submission to database");
                    return new JsonResult(new { success = false, message = "Failed to save your message. Please try again." });
                }

                // Send email
                var contactEmail = _configuration["Settings:ContactEmail"] ?? "devendrajoshi45@gmail.com";
                try
                {
                    var emailSubject = formData.FormSource == "FooterForm" 
                        ? $"Quick Service Request from {formData.Email}"
                        : $"New Contact Form Submission from {formData.FullName}";
                    
                    var emailBody = formData.FormSource == "FooterForm"
                        ? $@"
                            <h2>Quick Service Request</h2>
                            <p><strong>Source:</strong> Footer Form - Request Pest Control Service</p>
                            <p><strong>Email:</strong> {formData.Email}</p>
                            <p><strong>Submitted:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                            <p style='color: #ff9800; font-weight: bold;'>⚠️ This is a quick service request. Please follow up promptly!</p>
                        "
                        : $@"
                            <h2>New Contact Form Submission</h2>
                            <p><strong>Source:</strong> Main Contact Form</p>
                            <p><strong>Name:</strong> {formData.FullName}</p>
                            <p><strong>Email:</strong> {formData.Email}</p>
                            <p><strong>Message:</strong></p>
                            <p>{formData.Message}</p>
                        ";
                    
                    await _emailSender.SendEmailAsync(
                        to: contactEmail,
                        subject: emailSubject,
                        body: emailBody,
                        isBodyHtml: true
                    );
                    
                    _logger.LogInformation("Email sent successfully to {ContactEmail}", contactEmail);
                }
                catch (Exception emailEx)
                {
                    // Log but don't fail the request
                    _logger.LogError(emailEx, "Email send failed to {ContactEmail}. Error: {ErrorMessage}", contactEmail, emailEx.Message);
                }

                var successMessage = formData.FormSource == "FooterForm"
                    ? "Thank you for your service request! Our team has been notified and will contact you within 24 hours to discuss your pest control needs."
                    : "Thank you for contacting Calway Pest Control! Your message has been received and our team will get back to you within 24 hours.";

                return new JsonResult(new { success = true, message = successMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form");
                return new JsonResult(new { success = false, message = "An error occurred. Please try again." });
            }
        }
    }

    public class ContactFormData
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string FormSource { get; set; } = "ContactForm";
    }
}
