using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.DTOs.Movimentações.Get.Filtros;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Querys
{
    public sealed record RetornaMovimentacaoQuery(string IdUsuario,int IdConta,FiltroRetornoMovimentacao Filtros) : IRequest<Resultado<BaseGet<RetornaMovimentacaoDTO>>>;
}   
