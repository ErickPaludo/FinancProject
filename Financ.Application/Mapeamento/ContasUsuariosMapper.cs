using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Financ.Application.Mapeamento
{
    public static class ContasUsuariosMapper
    {
        public static RetornaUsuariosAssociadosDTO ParaUsuariosAssociadosDTO(ContaUsuario contaUsuario, Usuario usuario) =>
            new RetornaUsuariosAssociadosDTO(
                          contaUsuario.IdUsuario,
                          usuario.NomeCompleto,
                          usuario.Email,
                          contaUsuario.Acesso,
                          contaUsuario.Status,
                          contaUsuario.Expiracao,
                          contaUsuario.Expiracao.HasValue && contaUsuario.Expiracao < DateTime.UtcNow);

        public static BaseGet<RetornaContasDTO> ParaDTO(IEnumerable<ContaUsuario> contasUsuarios, FiltroContasUsuarioDTO? filtros)
        {
            List<RetornaContasDTO> listaContas = new List<RetornaContasDTO>();
            foreach (var contaUsuario in contasUsuarios)
            {
                listaContas.Add(new RetornaContasDTO(contaUsuario.Conta!.Id, contaUsuario.Conta.Titulo!, contaUsuario.Conta.Status, contaUsuario.Expiracao is not null ? contaUsuario.Expiracao < DateTime.UtcNow : null, contaUsuario.Expiracao));
            }

            return new BaseGet<RetornaContasDTO>(listaContas, new Meta { filtros = filtros });
        }
        public static BasePost<RetornaContasDTO> ParaDTO(ContaUsuario contaUsuario, FiltroContasUsuarioDTO? filtros)
        {
            return new BasePost<RetornaContasDTO>(new RetornaContasDTO(contaUsuario.Conta!.Id, contaUsuario.Conta.Titulo!, contaUsuario.Conta.Status, contaUsuario.Expiracao is not null ? contaUsuario.Expiracao < DateTime.UtcNow : null, contaUsuario.Expiracao));
        }
        public static RetornaCadastroContasUsuariosDTO ParaDTO(ContaUsuario contaUsuario) =>
            new RetornaCadastroContasUsuariosDTO(
                          contaUsuario.IdConta,
                          contaUsuario.Acesso,
                          contaUsuario.IdUsuario);

        public static BasePost<RetornaPostCadastroDTO> ParaDTO(Convite convite, ContaUsuario contaUsuario) =>
         new BasePost<RetornaPostCadastroDTO>(new RetornaPostCadastroDTO(convite.Aceito!.Value, ParaDTO(contaUsuario), convite.Observacao));
    }
}
