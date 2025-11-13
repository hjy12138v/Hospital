

using Microsoft.OpenApi.Models;

namespace His_Server.Api.Utility.SwaggerExt
{
    /// <summary>
    /// 这个类配置swagger
    /// </summary>
    public static class CustomSwaggerExt
    {
        /// <summary>
        /// 配置swagger
        /// </summary>
        /// <param name="builder"></param>
        public static void AddSwaggerExt(this WebApplicationBuilder builder)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = $"医院便民服务系统API",
                    Version = "v1",

                });
                //xml文档绝对路径
                var file = Path.Combine(AppContext.BaseDirectory,
                    "His_Server.Api.xml");
                //true,显示控制器层注释
                option.IncludeXmlComments(file, true);
                //对Action名称进行排序，如果有多个就可以看见效果
                option.OrderActionsBy(o => o.RelativePath);

                //添加安全定义--配置支持token授权机制
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入Token，格式为 Bearer your_token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //添加安全要求
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                  {
                        new OpenApiSecurityScheme
                        {
                            Reference =new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id ="Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });

            });
        }

        /// <summary>
        /// 使用swagger
        /// </summary>
        /// <param name="app"></param>
        public static void UseSwaggetExt(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

    }
}
