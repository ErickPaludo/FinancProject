

using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Interfaces.Repositorios.Movimentações.Fixas;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Financ.Infra.Data.Repositorios.Movimentações.Fixas
{
    public class MovimentacaoFixaRespositorio : BaseRepositorio<MovimentacaoFixa>, IMovimentacaoFixaRespositorio
    {
        private readonly AppContextoData _contexto;
        public MovimentacaoFixaRespositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public async Task<MovimentacaoFixa?> BuscaMovimentacaoFixaCompleta(Expression<Func<MovimentacaoFixa, bool>> predicado)
        {
            return await _contexto.MovimentacaoFixa
                 .Include(mf => mf.Movimentacao)
               .ThenInclude(cx => cx.CategoriasMovimentacao)
               .ThenInclude(cc => cc.Categoria)
           .Include(mf => mf.Movimentacao)
               .ThenInclude(m => m.Conta)
                .ThenInclude(c => c.ContaUsuarios)
                 .ThenInclude(cu => cu.Usuario)
           .Include(md => md.DiasFixosDiarios)
           .Include(mm => mm.Movimentacoes)
                .FirstOrDefaultAsync(predicado);
        }

        public IQueryable<MovimentacaoFixa> BuscaMovimentacoesFixaCompleta(Expression<Func<MovimentacaoFixa, bool>> predicado)
        {
            return _contexto.MovimentacaoFixa
           .Include(mf => mf.Movimentacao)
               .ThenInclude(cx => cx.CategoriasMovimentacao)
               .ThenInclude(cc => cc.Categoria)
           .Include(mf => mf.Movimentacao)
               .ThenInclude(m => m.Conta)
                .ThenInclude(c => c.ContaUsuarios)
                 .ThenInclude(cu => cu.Usuario)
           .Include(md => md.DiasFixosDiarios)
           .Include(mm => mm.Movimentacoes)
           .Where(predicado);
        }
    }
}
