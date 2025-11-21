
using His_Server.Api.Utility.SwaggerExt;
using SqlSugar;
using His_Server.Model.EntityMap;
using His_Server.BLL.Services;
using His_Server.DAL.Repositories;
using His_Server.BLL.Mapping;

namespace His_Server.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .SetPreflightMaxAge(TimeSpan.FromHours(1));
                });
                options.AddPolicy("DevOpen", policy =>
                {
                    policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                });
            });


            builder.Services.AddControllers();
            // 注册 AutoMapper 并扫描 BLL 中的 Profile
            builder.Services.AddAutoMapper(typeof(EntityProfiles).Assembly);
            // 配置SqlSugar数据库客户端（通过appsettings.json读取连接字符串）
            var connectionString = builder.Configuration.GetConnectionString("HisDbConnection");
            builder.Services.AddScoped<SqlSugarClient>(_ => new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            }));
            // 依赖注入：仓储与业务服务
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            // 新增 Notice 与 Medicine 的依赖注入
            builder.Services.AddScoped<INoticeRepository, NoticeRepository>();
            builder.Services.AddScoped<INoticeService, NoticeService>();
            builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();

            //配置swagger
            builder.AddSwaggerExt();

            WebApplication app = builder.Build();

            // 在应用启动时执行 CodeFirst 初始化用户、医生、科室表
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SqlSugarClient>();
                db.CodeFirst.InitTables(typeof(User), typeof(Doctor), typeof(Department), typeof(Notice), typeof(Medicine));
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("DevOpen");
               app.UseSwaggetExt();
            }
            else
            {
                app.UseCors("Default");
            }

                app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
