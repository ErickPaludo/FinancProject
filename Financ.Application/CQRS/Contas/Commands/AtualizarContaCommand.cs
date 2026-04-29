using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Domain.Entidades;
using Financ.Domain.Enums.ContasBancarias;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_Commands
{
    public sealed record AtualizarContaCommand(
          int IdConta,           
          string IdUsuario,      
          TiposStatusContas? Status,
          string? Titulo,
          string? Cor
      ) : IRequest<Resultado<BaseGet<RetornaContasDTO>>>;
}
