using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    public class CustomerType
    {
        public int CustomerTypeId { get; set; }
        public string CustomerTypeCode { get; set; }
        public string CustomerTypeNameAr { get; set; }
        public string CustomerTypeNameEn { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
