using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace CalwayPest.Domain
{
    public class AdminUser : AuditedAggregateRoot<Guid>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        protected AdminUser()
        {
        }

        public AdminUser(Guid id, string username, string password) : base(id)
        {
            Username = username;
            Password = password;
        }
    }
}
