using System;
using Volo.Abp.Domain.Entities;

namespace CalwayPest.Domain
{
    public class InvoiceItem : Entity<Guid>
    {
        public Guid InvoiceId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string GstType { get; set; } = string.Empty; // NoGST, GST5
        public decimal Amount { get; set; }
        
        public virtual Invoice? Invoice { get; set; }

        protected InvoiceItem()
        {
        }

        public InvoiceItem(
            Guid invoiceId,
            string description,
            int quantity,
            decimal unitPrice,
            string gstType)
        {
            Id = Guid.NewGuid();
            InvoiceId = invoiceId;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            GstType = gstType;
            Amount = quantity * unitPrice;
        }
    }
}
