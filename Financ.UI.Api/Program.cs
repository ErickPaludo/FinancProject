
using Financ.Infra.IoC;
using Financ.Ui.Api;
using Financ.UI.Api.Excessao;
using Serilog;
using Serilog.Events;

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
  
            builder.Host.UseSerilog(); //Responsavel pelos logs

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173", "https://financ-gestaofinanceira.github.io")
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });
            builder.Services.AddControllers();
            builder.Services.AddProblemDetails();

            builder.Services.AddExceptionHandler<ExcessaoGlobal>();
            var app = builder.Build();
            app.UseExceptionHandler();
            // app.UseMiddleware<MiddlewareErroInterno>();
            app.MigrateDatabase();
        
            app.UseSwagger();
            app.UseSwaggerUI();
          
            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
     
        }
    }
}
