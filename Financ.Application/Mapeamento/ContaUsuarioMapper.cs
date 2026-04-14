using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Financ.Application.Mapeamento
{
    public static class ContaUsuarioMapper
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
        public static RetornaContaUsuarioDTO ParaUsuarioDTO(ContaUsuario contaUsuario) =>
            new RetornaContaUsuarioDTO(
                          contaUsuario.Id,
                          contaUsuario.Usuario.Id,
                          contaUsuario.Usuario.Email,
                          contaUsuario.Usuario.PrimeiroNome,
                          contaUsuario.Usuario.SegundoNome,
                          contaUsuario.Usuario.NomeCompleto);

        public static BaseGetList<RetornaContasDTO> ParaDTO(IEnumerable<ContaUsuario> contasUsuarios, IEnumerable<Movimentacao> movimentacaos, FiltroContasUsuarioDTO? filtros)
        {
            List<RetornaContasDTO> listaContas = new List<RetornaContasDTO>();
            foreach (var contaUsuario in contasUsuarios)
            {
                listaContas.Add(new RetornaContasDTO(
                    contaUsuario.Conta!.Id,
                    contaUsuario.Conta.Titulo!,
                    contaUsuario.Conta!.Cor.Valor,
                    contaUsuario.Conta.Status,
                    contaUsuario.Conta.Saldo,
                    movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Entrada).Sum(m => m.Valor),
                    movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Saida).Sum(m => m.Valor),
                    contaUsuario.Expiracao is not null ? contaUsuario.Expiracao < DateTime.UtcNow : null,
                    contaUsuario.Expiracao));
            }

            return new BaseGetList<RetornaContasDTO>(listaContas, new Meta { tamanho = listaContas.Count, filtros = filtros });
        }
        public static BasePost<RetornaContasDTO> ParaDTO(ContaUsuario contaUsuario, IEnumerable<Movimentacao> movimentacaos, FiltroContasUsuarioDTO? filtros)
        {
            return new BasePost<RetornaContasDTO>(
                new RetornaContasDTO(
                    contaUsuario.Conta!.Id,
                    contaUsuario.Conta.Titulo!,
                    contaUsuario.Conta!.Cor.Valor,
                    contaUsuario.Conta.Status,
                    contaUsuario.Conta.Saldo,
                    movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Entrada).Sum(m => m.Valor),
                    movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Saida).Sum(m => m.Valor),
                    contaUsuario.Expiracao is not null ? contaUsuario.Expiracao < DateTime.UtcNow : null, contaUsuario.Expiracao));
        }
        public static BasePost<RetornaContasDTO> ParaDTO(ContaUsuario contaUsuario, FiltroContasUsuarioDTO? filtros)
        {
            return new BasePost<RetornaContasDTO>(
                new RetornaContasDTO(
                    contaUsuario.Conta!.Id,
                    contaUsuario.Conta.Titulo!,
                    contaUsuario.Conta!.Cor.Valor,
                    contaUsuario.Conta.Status,
                    contaUsuario.Conta.Saldo,
                   0, 0,
                    contaUsuario.Expiracao is not null ? contaUsuario.Expiracao < DateTime.UtcNow : null, contaUsuario.Expiracao));
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
