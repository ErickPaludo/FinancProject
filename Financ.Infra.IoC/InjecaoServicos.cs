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

using Financ.Infra.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Financ.Domain.Interfaces.Autenticação;
using Financ.Application.CQRS.Handler;
using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.CQRS.Querys;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.Usuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Application.DTOs.Convites.Get;


namespace Financ.Infra.IoC
{
    public static class InjecaoServicos
    {
        public static void ConfigurarInjecaoServicos(this IServiceCollection services)
        {
            services.AddScoped<IUsuariosIdentityServicos, UsuariosIdentityServicos>();

            services.AddScoped<IRequestHandler<CriarContaCommand, Resultado<RetornaContasDTO>>, CriarContaHandler>();
            services.AddScoped<IRequestHandler<AtualizarContaCommand, Resultado<RetornaContasDTO>>, AtualizarContasHandler>();
            services.AddScoped<IRequestHandler<RetornaContaQuery, Resultado<List<RetornaContasDTO>>>, RetornaContasHandler>();

            // 2. Contexto de Contas de Usuários (Vínculos)
            services.AddScoped<IRequestHandler<IncluiUsuarioContaCommand, Resultado<RetornaPostCadastro>>, IncluirUsuarioContaHandler>();
            services.AddScoped<IRequestHandler<AtualizarContaUsuarioCommand, Resultado<RetornaCadastroContasUsuariosDTO>>, AtualizarContaUsuarioHandler>();
            services.AddScoped<IRequestHandler<RetornaUsuariosAssociadosQuery, Resultado<List<RetornaUsuariosAssociadosDTO>>>, RetornaUsuariosAssociadosHandler>();

            // 3. Contexto de Usuários e Autenticação
            services.AddScoped<IRequestHandler<CadastraUsuarioCommand, Resultado<string>>, CadastraUsuarioHandler>();
            services.AddScoped<IRequestHandler<AutenticadoUsuarioCommand, Resultado<RetornaTokenDTO>>, AutenticadoUsuarioHandler>();
            services.AddScoped<IRequestHandler<RetornaUsuarioPorIdQuery, Resultado<RetornaUsuarioDTO>>, RetornaUsuarioHandler>();
            
            services.AddScoped<IRequestHandler<CriaConviteCommand, Resultado<GetCriaConviteDTO>>, CriaConviteHandler>();

            services.AddIdentity<UsuarioIdentity, IdentityRole>()
           .AddEntityFrameworkStores<AppContextoData>()
           .AddDefaultTokenProviders();
        }
    }
}
