using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Interfaces.Repositorios.Movimentações.Fixas;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.Movimentações.Fixas
{
    public class MovimentacaoFixasDiariasRespositorio : BaseRepositorio<MovimentacaoFixaDiaria>, IMovimentacaoFixaDiariaRespositorio
    {
        private readonly AppContextoData _contexto;
        public MovimentacaoFixasDiariasRespositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }
    }
}
