using Financ.Application.Interfaces;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Repositorios.ContasBancarias;
using Financ.Domain.Interfaces.Repositorios.Movimentações;
using Financ.Domain.Interfaces.Repositorios.Segurança;
using Financ.Domain.Interfaces.Repositorios.Usuarios;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios;
using Financ.Infra.Data.Repositorios.ContasBancarias;
using Financ.Infra.Data.Repositorios.Movimentações;
using Financ.Infra.Data.Repositorios.Segurança;
using Financ.Infra.Data.Repositorios.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


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
        private IMovimentacaoRepositorio _movimentacaoRepositorio;
        private ICategoriaRepositorio _categoriaRepositorio;
        private IMovimentacaoCategoriaRepositorio _movimentacaoCategoriaRepositorio;

        private readonly IConfiguration _configuration;
        public UnitOfWork(AppContextoData contexto, IConfiguration configuration)
        {
            _contexto = contexto;
            _configuration = configuration;
        }
        public IContasRepositorio contasRepositorio { get { return _contasRepositorio = _contasRepositorio ?? new ContasRepositorio(_contexto); } }
        public IContasUsuariosRepositorio contasUsuariosRepositorio { get { return _contasUsuariosRepositorio = _contasUsuariosRepositorio ?? new ContasUsuariosRepositorio(_contexto); } }
        public IConvitesRepostorio convitesRepostorio { get { return _convitesRepostorio = _convitesRepostorio ?? new ConvitesRepositorio(_contexto); } }
        public IUsuariosRepositorio usuariosRepostorio { get { return _usuariosRepostorio = _usuariosRepostorio ?? new UsuariosRepositorio(_contexto); } }
        public IAutenticacoesRepositorio autenticacoesRepositorio { get { return _autenticacoesRepositorio = _autenticacoesRepositorio ?? new AutenticacoesRepositorio(_contexto); } }
        public IMovimentacaoRepositorio movimentacaoRepositorio { get { return _movimentacaoRepositorio = _movimentacaoRepositorio ?? new MovimentacaoRepositorio(_contexto); } }
        public ICategoriaRepositorio categoriaRepositorio { get { return _categoriaRepositorio = _categoriaRepositorio ?? new CategoriaRepositorio(_contexto); } }
        public IMovimentacaoCategoriaRepositorio movimentacaoCategoriaRepositorio { get { return _movimentacaoCategoriaRepositorio = _movimentacaoCategoriaRepositorio ?? new MovimentacaoCategoriaRepositorio(_contexto); } }
        public async Task Commit()
        {
            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Conflito de concorrência. Os dados foram alterados por outro processo.");
            }
        }
    }
}
