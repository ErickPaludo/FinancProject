using Financ.Application.Interfaces;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Repositorios;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios;
using Financ.Infra.Data.Repositorios.Leitura;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppContextoData _contexto;
        private IContasRepositorio _contasRepositorio;
        private IContasUsuariosRepositorio _contasUsuariosRepositorio;
        private IConvitesRepostorio _convitesRepostorio;
        private IUsuariosRepositorio _usuariosRepostorio;
        private IAutenticacoesRepositorio _autenticacoesRepositorio;

        private readonly IConfiguration _configuration;
        public UnitOfWork(AppContextoData contexto,IConfiguration configuration)
        {
            _contexto = contexto;
            _configuration = configuration;
        }
        public IContasRepositorio contasRepositorio { get { return _contasRepositorio = _contasRepositorio ?? new ContasRepositorio(_contexto); } }
        public IContasUsuariosRepositorio contasUsuariosRepositorio { get { return _contasUsuariosRepositorio = _contasUsuariosRepositorio ?? new ContasUsuariosRepositorio(_contexto); } }
        public IConvitesRepostorio convitesRepostorio { get { return _convitesRepostorio = _convitesRepostorio ?? new ConvitesRepositorio(_contexto); } }  
        public IUsuariosRepositorio usuariosRepostorio { get { return _usuariosRepostorio = _usuariosRepostorio ?? new UsuariosRepositorio(_contexto); } }
        public IAutenticacoesRepositorio autenticacoesRepositorio { get { return _autenticacoesRepositorio = _autenticacoesRepositorio ?? new AutenticacoesRepositorio(_contexto); } }

        public async Task Commit()
        {
            await _contexto.SaveChangesAsync();
        }
    }
}
