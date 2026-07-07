using System;

namespace EFA.Domain.Entities
{
    public class VendorContact
    {
        public int VendorContactId { get; set; }
        public int VendorId { get; set; }
        
        public string ContactName { get; set; } = null!;
        public string? ContactTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsDefault { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual Vendor? Vendor { get; set; }
    }
}
