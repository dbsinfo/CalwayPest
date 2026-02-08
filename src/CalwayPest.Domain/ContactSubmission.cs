using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CalwayPest
{
    public class ContactSubmission : AuditedAggregateRoot<Guid>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string FormSource { get; set; }

        protected ContactSubmission()
        {
        }

        public ContactSubmission(Guid id, string fullName, string email, string message, string formSource = "ContactForm")
            : base(id)
        {
            FullName = fullName;
            Email = email;
            Message = message;
            FormSource = formSource;
        }
    }
}
