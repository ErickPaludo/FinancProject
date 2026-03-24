using Financ.Application.Interfaces.Segurança;
using Financ.Application.Services.Segurança;
using Financ.Infra.Security.Configurações.Segurança;
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
            services.Configure<SegurancaConfig>(configuration.GetSection("Auth"));
            services.AddScoped<ISegurancaServico, SegurancaServico>();
        }
    }
}
