using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace His_Server.Model.EntityMap
{
    [SugarTable("User")]
    public class User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(255)")]
        public string Name { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(10)")]
        public string Gender { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(20)")]
        public string PhoneNumber { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(255)")]
        public string Email { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(255)")]
        public string Password { get; set; }

        public int RoleId { get; set; }

        [SugarColumn(ColumnDataType = "DATE")]
        public DateTime DateOfBirth { get; set; }
    }
}
