using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Interfaces.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Interfaces.Repositorios.Movimentações
{
    public interface IMovimentacaoRepositorio : IBaseRepositorio<Movimentacao>
    {
        Task<Movimentacao?> BuscaMovimentacaoUnicaComContasUsuarios(Expression<Func<Movimentacao, bool>> predicado);
        IQueryable<Movimentacao> BuscaMovimentacaoComContasUsuarios();
        Task<decimal> SomaTotalConcluidas(int idConta,DateTime dtMax);
        Task<decimal> SomaTotalPendentes(int idConta,DateTime dtMax);
    }
}
