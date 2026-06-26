using Financ.Domain.Interfaces.Repositorios;
using Financ.Domain.Interfaces;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios;
using Financ.Infra.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Financ.Infra.IoC
{
    public static class InjecaoBibliotecas
    {
        public static void ConfigurarInjecaoBibliotecas(this IServiceCollection services )
        {
            services.AddSimpleMediator();
            services.AddScoped<IMediator, Mediator>();

            var outputTemplate = "{Timestamp} [{Level}] {Message}{NewLine}{Exception}{NewLine}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: outputTemplate)
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}
