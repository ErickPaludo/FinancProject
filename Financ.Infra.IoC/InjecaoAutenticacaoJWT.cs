using Financ.Application.Interfaces.Autenticação;
using Financ.Application.Services.Autenticação;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Repositorios.Segurança;
using Financ.Infra.Data;
using Financ.Infra.Security.Configurações.Autenticação;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.IoC
{
    public static class InjecaoAutenticacaoJWT
    {
        public static IServiceCollection ConfigurarInjecaoAutenticaoJWT(this IServiceCollection services,
           IConfiguration configuration)
        {
            services.Configure<AutenticaoConfig>(configuration.GetSection("TokenJWT"));

            services.AddScoped<IAutenticacaoServico, AutenticacaoServico>();

            var secretKey = configuration.GetValue<string>("TokenJWT:SecretKeyJWT");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                        var sid = context.Principal.FindFirst("sid")?.Value;

                        var repo = context.HttpContext.RequestServices
                            .GetRequiredService<IAutenticacoesRepositorio>();

                        var usuario = await repo.BuscarObjetoUnico(x => x.IdUsuario == userId);

                        if (usuario == null || usuario.RefreshToken != sid || usuario.Revoke)
                        {
                            context.Fail("Sessão inválida");
                        }
                    }
                };
            });

            return services;
        }
    }
}
