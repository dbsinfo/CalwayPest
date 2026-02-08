using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace CalwayPest.Domain
{
    public class Invoice : AuditedAggregateRoot<Guid>
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public decimal SubtotalAmount { get; set; }
        public decimal GstTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Sent, Paid, Cancelled
        public DateTime? SentDate { get; set; }
        public DateTime? PaidDate { get; set; }
        
        public virtual ICollection<InvoiceItem> Items { get; set; }

        protected Invoice()
        {
            Items = new List<InvoiceItem>();
        }

        public Invoice(
            Guid id,
            string invoiceNumber,
            DateTime invoiceDate,
            string customerName,
            string customerEmail,
            string customerPhone,
            string customerAddress,
            decimal subtotalAmount,
            decimal gstTotalAmount,
            decimal totalAmount,
            string status = "Draft")
            : base(id)
        {
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            CustomerAddress = customerAddress;
            SubtotalAmount = subtotalAmount;
            GstTotalAmount = gstTotalAmount;
            TotalAmount = totalAmount;
            Status = status;
            Items = new List<InvoiceItem>();
        }
    }
}
