using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Financ.Application.Mapeamento
{
    public static class ContasUsuariosMapper
    {
        public static RetornaUsuariosAssociadosDTO ParaDTO(ContasUsuarios contaUsuario, Usuario usuario) =>
            new RetornaUsuariosAssociadosDTO(
                          contaUsuario.IdUsuario,
                          usuario.NomeCompleto,
                          usuario.Email,
                          contaUsuario.Acesso,
                          contaUsuario.Status);
        public static RetornaCadastroContasUsuariosDTO ParaDTO(ContasUsuarios contaUsuario) =>
            new RetornaCadastroContasUsuariosDTO(
                          contaUsuario.IdConta,
                          contaUsuario.Acesso,
                          contaUsuario.IdUsuario);

        public static RetornaPostCadastroDTO ParaDTO(Convites convite, ContasUsuarios contaUsuario) =>
         new RetornaPostCadastroDTO(convite.Aceito!.Value,ParaDTO(contaUsuario),convite.Observacao);
    }
}
