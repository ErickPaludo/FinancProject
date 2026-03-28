using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
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
                          contaUsuario.Status,
                          contaUsuario.Expiracao,
                          contaUsuario.Expiracao.HasValue && contaUsuario.Expiracao < DateTime.UtcNow);

        public static Data<RetornaContasUsuariosDTO> ParaDTO(IEnumerable<ContasUsuarios> contasUsuarios, FiltroContasUsuarioDTO? filtros)
        {
            List<RetornaContasUsuariosDTO> listaContas = new List<RetornaContasUsuariosDTO>();
            foreach (var contaUsuario in contasUsuarios)
            {
                listaContas.Add(new RetornaContasUsuariosDTO(new RetornaContasDTO(contaUsuario.Conta!.Id, contaUsuario.Conta.Titulo!, contaUsuario.Conta.Status), contaUsuario.Expiracao is not null ? contaUsuario.Expiracao < DateTime.UtcNow : null, contaUsuario.Expiracao));
            }

            return new Data<RetornaContasUsuariosDTO>(listaContas,new Meta { filtros = filtros});
        }
        public static RetornaCadastroContasUsuariosDTO ParaDTO(ContasUsuarios contaUsuario) =>
            new RetornaCadastroContasUsuariosDTO(
                          contaUsuario.IdConta,
                          contaUsuario.Acesso,
                          contaUsuario.IdUsuario);

        public static RetornaPostCadastroDTO ParaDTO(Convites convite, ContasUsuarios contaUsuario) =>
         new RetornaPostCadastroDTO(convite.Aceito!.Value,ParaDTO(contaUsuario),convite.Observacao);
    }
}
