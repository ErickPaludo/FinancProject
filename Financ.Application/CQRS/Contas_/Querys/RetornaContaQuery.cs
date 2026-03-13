using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.Contas.Get.Filtros;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_.Querys
{
    public class RetornaContaQuery : IRequest<Resultado<List<RetornaContasDTO>>>
    {
        public string IdUsuario { get; private set; }
        public FiltroContaDTO? Filtros { get; private set; }
        public RetornaContaQuery(string idUsuario, FiltroContaDTO? filtros)
        {
            IdUsuario = idUsuario;
            Filtros = filtros;
        }
    }
}
