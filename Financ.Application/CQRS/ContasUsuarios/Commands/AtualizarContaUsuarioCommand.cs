using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Domain.Enums.ContasBancarias;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_Usuarios.Commands
{
    public record AtualizarContaUsuarioCommand(string idUsuarioSolicitante,string idUsuarioAlterado, int idConta, TiposAcessos? acesso, StatusContasUsuario? status,int? expiracao,bool? removerExpiracao) : IRequest<Resultado<RetornaCadastroContasUsuariosDTO>>;
}
