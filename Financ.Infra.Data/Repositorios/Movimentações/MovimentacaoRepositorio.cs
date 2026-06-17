using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Interfaces.Repositorios.Movimentações;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.Movimentações
{
    public class MovimentacaoRepositorio : BaseRepositorio<Movimentacao>, IMovimentacaoRepositorio
    {
        private readonly AppContextoData _contexto;
        public MovimentacaoRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public async Task<Movimentacao?> BuscaMovimentacaoUnicaComContasUsuarios(Expression<Func<Movimentacao, bool>> predicado)
        {
            return await _contexto.Movimentacao
                .Include(mc => mc.CategoriasMovimentacao)
                  .ThenInclude(c => c.Categoria)
                .Include(u => u.ContaUsuarioCriador).ThenInclude(u => u.Usuario)
                .Include(u => u.ContaUsuarioExecutor).ThenInclude(u => u!.Usuario)
                .Include(c => c.Conta.ContaUsuarios)
                .FirstOrDefaultAsync(predicado);
        }
        public IQueryable<Movimentacao> BuscaMovimentacaoComContasUsuarios()
        {
            return _contexto.Movimentacao
                           .Include(mc => mc.CategoriasMovimentacao)
                            .ThenInclude(c => c.Categoria)
                           .Include(mf => mf.Fixa)
                           .Include(u => u.ContaUsuarioCriador).ThenInclude(u => u.Usuario)
                           .Include(u => u.ContaUsuarioExecutor).ThenInclude(u => u!.Usuario)
                           .Include(c => c.Conta.ContaUsuarios);
        }

        public async Task<decimal> SomaTotalPendentes(int idConta, DateTime dtMax)
        {
            return await _contexto.Movimentacao
                         .Where(x => x.IdConta == idConta && x.DthrMovimentacao <= dtMax && x.Status == StatusMovimentacao.Pendente)
                         .SumAsync(x => x.Tipo == TipoMovimentacao.Entrada ? x.Valor :
                                        x.Tipo == TipoMovimentacao.Saida ? -x.Valor : 0);
        }

        public async Task<decimal> SomaTotalConcluidas(int idConta, DateTime dtMax)
        {
            var teste = await _contexto.Movimentacao
                                  .Where(x => x.IdConta == idConta && x.DthrConclusao!.Value.Date <= dtMax.Date && x.Status == StatusMovimentacao.Concluido && x.Tipo == TipoMovimentacao.Entrada)
                                  .SumAsync(x => x.Valor);

            var testee = await _contexto.Movimentacao
                                 .Where(x => x.IdConta == idConta && x.DthrConclusao!.Value.Date <= dtMax.Date && x.Status == StatusMovimentacao.Concluido && x.Tipo == TipoMovimentacao.Saida)
                                 .SumAsync(x => x.Valor);

            return await _contexto.Movimentacao
                                  .Where(x => x.IdConta == idConta && x.DthrConclusao <= dtMax && x.Status == StatusMovimentacao.Concluido)
                                  .SumAsync(x => x.Tipo == TipoMovimentacao.Entrada ? x.Valor :
                                                 x.Tipo == TipoMovimentacao.Saida ? -x.Valor : 0);
        }
    }
}
