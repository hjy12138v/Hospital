using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Server.Model.EntityDto
{
    public class MedicineDto
    {
        public int MedicineId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? Type { get; set; }
    }
}
