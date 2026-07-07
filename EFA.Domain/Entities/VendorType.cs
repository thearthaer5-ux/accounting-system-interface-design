using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class VendorType
    {
        public int VendorTypeId { get; set; }
        public string VendorTypeCode { get; set; } = null!;
        public string VendorTypeNameAr { get; set; } = null!;
        public string? VendorTypeNameEn { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
    }
}
