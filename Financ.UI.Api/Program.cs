
using Financ.Infra.IoC;
using Financ.Ui.Api;
using Financ.UI.Api.Middleware;

namespace Financ.UI.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.ConfigurarInjecaoSwagger(builder.Configuration);
            builder.Services.ConfigurarInjecaoPassword(builder.Configuration);
            builder.Services.ConfigurarInjecaoAutenticaoJWT(builder.Configuration);
            builder.Services.ConfigurarInjecaoInfraestrutura(builder.Configuration);
            builder.Services.ConfigurarInjecaoServicos();
            builder.Services.ConfigurarInjecaoBibliotecas();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var MyAllowSpecificOrigins = "_MyAllowSubdomainPolicy";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173", "http://68.211.177.67:5000")
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });
            builder.Services.AddControllers();

            var app = builder.Build();
            app.UseMiddleware<MiddlewareErroInterno>();
            app.MigrateDatabase();
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
