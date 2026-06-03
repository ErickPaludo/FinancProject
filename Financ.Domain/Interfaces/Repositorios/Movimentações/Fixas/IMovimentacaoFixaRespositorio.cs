using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Interfaces.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Interfaces.Repositorios.Movimentações.Fixas
{
    public interface IMovimentacaoFixaRespositorio : IBaseRepositorio<MovimentacaoFixa>
    {
        public IQueryable<MovimentacaoFixa> BuscaMovimentacoesFixaCompleta(Expression<Func<MovimentacaoFixa, bool>> predicado);
        public Task<MovimentacaoFixa?> BuscaMovimentacaoFixaCompleta(Expression<Func<MovimentacaoFixa, bool>> predicado);
    }
}
