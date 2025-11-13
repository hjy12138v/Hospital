using His_Server.Model.EntityMap;
using SqlSugar;

namespace His_Server.DAL
{
    public class DbContext
    {
        private static SqlSugarClient _db;
        public static SqlSugarClient Db
        {
            get
            {
                if (_db == null)
                {
                    var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["HisDbConnection"].ConnectionString;
                    _db = new SqlSugarClient(new ConnectionConfig()
                    {
                        ConnectionString = connectionString,
                        DbType = DbType.SqlServer,
                        IsAutoCloseConnection = true,
                        InitKeyType = InitKeyType.Attribute
                    });
                }
                return _db;
            }

        }

        public static void InitDatabase()
        {
            Db.CodeFirst.InitTables<User>();
            Db.CodeFirst.InitTables<Doctor>();
            Db.CodeFirst.InitTables<Department>();
            Db.CodeFirst.InitTables<Notice>();
            Db.CodeFirst.InitTables<Medicine>();
        }
    }
}
