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
        public static RetornaUsuariosAssociadosDTO ParaUsuariosAssociadosDTO(ContaUsuario contaUsuario) =>
            new RetornaUsuariosAssociadosDTO(
                          contaUsuario.Id,
                          contaUsuario.IdUsuario,
                          contaUsuario.Usuario.NomeCompleto,
                          contaUsuario.Usuario.Email,
                          contaUsuario.Acesso,
                          contaUsuario.Status,
                          contaUsuario.Expiracao.HasValue ? DateTime.SpecifyKind(contaUsuario.Expiracao.Value, DateTimeKind.Utc) : null,
                          contaUsuario.Expiracao.HasValue && contaUsuario.Expiracao < DateTime.UtcNow);
        public static RetornaContaUsuarioDTO ParaUsuarioDTO(ContaUsuario contaUsuario) =>
            new RetornaContaUsuarioDTO(
                          contaUsuario.Id,
                          contaUsuario.Usuario.Id,
                          contaUsuario.Usuario.Email,
                          contaUsuario.Usuario.PrimeiroNome,
                          contaUsuario.Usuario.SegundoNome,
                          contaUsuario.Usuario.NomeCompleto);
        public static List<RetornaUsuariosAssociadosDTO> ParaUsuarioDTO(List<ContaUsuario> contaUsuario) =>
            contaUsuario.Select(ParaUsuariosAssociadosDTO).ToList();


        public static BaseGet<RetornarContasDTO> ParaDTO(IEnumerable<ContaUsuario> contasUsuarios, IEnumerable<Movimentacao> movimentacaos, FiltroContasUsuarioDTO? filtros)
        {
            List<ContasDTO> listaContas = new List<ContasDTO>();
            foreach (var contaUsuario in contasUsuarios)
            {
                decimal entradaPendente = movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Entrada).Sum(m => m.Valor);
                decimal saidaPendente = movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Saida).Sum(m => m.Valor);
                decimal saldoProjetado = contaUsuario.Conta.Saldo + entradaPendente - saidaPendente;
                listaContas.Add(new ContasDTO(
                    contaUsuario.Conta!.Id,
                    contaUsuario.Conta.Titulo!,
                    contaUsuario.ContaFavorita,
                    contaUsuario.AutoSoma,
                    contaUsuario.Conta!.Cor.Valor,
                    contaUsuario.Conta.Status,
                    contaUsuario.Conta.Saldo,
                    saldoProjetado,
                    entradaPendente,
                    saidaPendente,
                     contaUsuario.Expiracao.HasValue ? DateTime.SpecifyKind(contaUsuario.Expiracao.Value, DateTimeKind.Utc) : null));
            }

            var listaSoma = listaContas.Where(x => x.SomaSaldo);

            return new BaseGet<RetornarContasDTO>(
                new RetornarContasDTO(
                    listaSoma.Sum(x => x.SaldoAtual),
                    listaSoma.Sum(x => x.SaldoProjetado),
                    listaSoma.Sum(x => x.EntradaPendente),
                    listaSoma.Sum(x => x.SaidaPendente),
                    listaContas), 
                new Meta { tamanho = listaContas.Count, filtros = filtros });
        }

        public static BaseGet<ContasDTO> ParaGetDTO(ContaUsuario contaUsuario, IEnumerable<Movimentacao> movimentacaos)
        {
          
                decimal entradaPendente = movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Entrada).Sum(m => m.Valor);
                decimal saidaPendente = movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Saida).Sum(m => m.Valor);
                decimal saldoProjetado = contaUsuario.Conta.Saldo + entradaPendente - saidaPendente;

               ContasDTO dto = new ContasDTO(
                    contaUsuario.Conta!.Id,
                    contaUsuario.Conta.Titulo!,
                    contaUsuario.ContaFavorita,
                    contaUsuario.AutoSoma,
                    contaUsuario.Conta!.Cor.Valor,
                    contaUsuario.Conta.Status,
                    contaUsuario.Conta.Saldo,
                    saldoProjetado,
                    entradaPendente,
                    saidaPendente,
                    contaUsuario.Expiracao.HasValue ? DateTime.SpecifyKind(contaUsuario.Expiracao.Value, DateTimeKind.Utc):null);
            

            return new BaseGet<ContasDTO>(dto);
        }
        public static BasePost<ContasDTO> ParaPostDTO(ContaUsuario contaUsuario, IEnumerable<Movimentacao> movimentacaos)
        {
            decimal entradaPendente = movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Entrada).Sum(m => m.Valor);
            decimal saidaPendente = movimentacaos.Where(m => m.IdConta == contaUsuario.Conta.Id && m.Tipo == TipoMovimentacao.Saida).Sum(m => m.Valor);
            decimal saldoProjetado = contaUsuario.Conta.Saldo + entradaPendente - saidaPendente;

            return new BasePost<ContasDTO>(
               new ContasDTO(
                    contaUsuario.Conta!.Id,
                    contaUsuario.Conta.Titulo!,
                    contaUsuario.ContaFavorita,
                    contaUsuario.AutoSoma,
                    contaUsuario.Conta!.Cor.Valor,
                    contaUsuario.Conta.Status,
                    contaUsuario.Conta.Saldo,
                    saldoProjetado,
                    entradaPendente,
                    saidaPendente,
                     contaUsuario.Expiracao.HasValue ? DateTime.SpecifyKind(contaUsuario.Expiracao.Value, DateTimeKind.Utc) : null));
        }

        public static BasePost<ContasDTO> ParaDTO(ContaUsuario contaUsuario, FiltroContasUsuarioDTO? filtros)
        {
            return new BasePost<ContasDTO>(
                new ContasDTO(
                    contaUsuario.Conta!.Id,
                    contaUsuario.Conta.Titulo!,
                    contaUsuario.ContaFavorita,
                    contaUsuario.AutoSoma,
                    contaUsuario.Conta!.Cor.Valor,
                    contaUsuario.Conta.Status,
                    contaUsuario.Conta.Saldo,
                    0, 0, 0,
                     contaUsuario.Expiracao.HasValue ? DateTime.SpecifyKind(contaUsuario.Expiracao.Value, DateTimeKind.Utc) : null));
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
