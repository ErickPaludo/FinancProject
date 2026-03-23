using Financ.Application.Configurações;
using Financ.Application.Interfaces;
using Financ.Application.Services.Segurança;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.IoC
{
    public static class InjecaoPassword
    {
        public static void ConfigurarInjecaoPassword(this IServiceCollection services,
           IConfiguration configuration)
        {
            services.Configure<PassConfig>(configuration.GetSection("Auth"));
            services.AddScoped<IPassService, PassService>();
        }
    }
}
