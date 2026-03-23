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
using Microsoft.AspNetCore.Identity;
using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.Usuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Application.DTOs.Convites.Get;
using Financ.Application.CQRS.Contas_.Commands;
using Financ.Application.CQRS.Contas_Commands;
using Financ.Application.CQRS.Contas_.Querys;
using Financ.Application.CQRS.Contas_.Handler;
using Financ.Application.CQRS.Contas_Usuarios.Commands;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.CQRS.Contas_Usuarios.Handler;
using Financ.Application.CQRS.Usuarios.Commands;
using Financ.Application.CQRS.Usuarios.Handler;
using Financ.Application.CQRS.Usuarios.Querys;
using Financ.Application.CQRS.Convites_.Commands;
using Financ.Application.CQRS.Convites_.Handler;
using Financ.Application.CQRS.Autenticação.Handler;
using Financ.Application.CQRS.Autenticação.Commands;


namespace Financ.Infra.IoC
{
    public static class InjecaoServicos
    {
        public static void ConfigurarInjecaoServicos(this IServiceCollection services)
        {

            services.AddScoped<IRequestHandler<CriarContaCommand, Resultado<RetornaContasDTO>>, CriarContaHandler>();
            services.AddScoped<IRequestHandler<AtualizarContaCommand, Resultado<RetornaContasDTO>>, AtualizarContasHandler>();
            services.AddScoped<IRequestHandler<RetornaContaQuery, Resultado<List<RetornaContasDTO>>>, RetornaContasHandler>();

            // 2. Contexto de Contas de Usuários (Vínculos)
            services.AddScoped<IRequestHandler<AceitaConviteCommand, Resultado<RetornaPostCadastroDTO>>, AceitaConviteHandler>();
            services.AddScoped<IRequestHandler<AtualizarContaUsuarioCommand, Resultado<RetornaCadastroContasUsuariosDTO>>, AtualizarContaUsuarioHandler>();
            services.AddScoped<IRequestHandler<RetornaUsuariosAssociadosQuery, Resultado<List<RetornaUsuariosAssociadosDTO>>>, RetornaUsuariosAssociadosHandler>();

            // 3. Contexto de Usuários e Autenticação
            services.AddScoped<IRequestHandler<CadastraUsuarioCommand, Resultado<string>>, CadastraUsuarioHandler>();
            services.AddScoped<IRequestHandler<AutenticacaoCommand, Resultado<RetornaTokenDTO>>, AutenticacaoHandler>();
            services.AddScoped<IRequestHandler<RetornaUsuarioPorIdQuery, Resultado<RetornaUsuarioDTO>>, RetornaUsuarioHandler>();
            
            services.AddScoped<IRequestHandler<CriaConviteCommand, Resultado<GetCriaConviteDTO>>, CriaConviteHandler>();

        }
    }
}
