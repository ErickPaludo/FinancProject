using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
using Financ.Domain.Enums;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_Usuarios.Querys
{
    public record RetornaUsuariosAssociadosQuery(int IdConta, string? IdUsuario, FiltroUsuarioAssociado? filtroConta) : IRequest<Resultado<BaseGetList<RetornaUsuariosAssociadosDTO>>>;
}
