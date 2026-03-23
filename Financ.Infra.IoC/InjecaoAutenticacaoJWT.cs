using Financ.Application.Configurações;
using Financ.Application.Interfaces;
using Financ.Application.Services.Autenticação;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Repositorios;
using Financ.Infra.Data;
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
            services.Configure<TokenConfig>(configuration.GetSection("TokenJWT"));

            services.AddScoped<ITokenService, TokenService>();

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
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["TokenJWT:SecretKeyJWT"])),
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
