using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace His_Server.Model.EntityMap
{
    [SugarTable("Medicine")]
    public class Medicine
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int MedicineId { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(255)")]
        public string Name { get; set; }

        [SugarColumn(ColumnDataType = "TEXT")]
        public string Description { get; set; }

        public int Stock { get; set; }

        [SugarColumn(ColumnDataType = "DECIMAL(10, 2)")]
        public decimal UnitPrice { get; set; }

        [SugarColumn(ColumnDataType = "DATE")]
        public DateTime ExpiryDate { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(50)")]
        public string Type { get; set; }
    }
}
