using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_Usuarios.Querys
{
    public class RetornaContaUsuariosQuery : IRequest<Resultado<BaseGetList<RetornaContasDTO>>>
    {
        public string IdUsuario { get; private set; }
        public FiltroContasUsuarioDTO? Filtros { get; private set; }
        public RetornaContaUsuariosQuery(string idUsuario, FiltroContasUsuarioDTO? filtros)
        {
            IdUsuario = idUsuario;
            Filtros = filtros;
        }
    }
}
