using Financ.Domain.Enums.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Get
{
    public record RetornaUsuariosAssociadosDTO(int idContaUsuario,string IdUsuario,string Nome,string Email,TiposAcessos Permissao, TipoStatusContasUsuario Status,DateTime? Expiracao,bool? Expirado);
}
