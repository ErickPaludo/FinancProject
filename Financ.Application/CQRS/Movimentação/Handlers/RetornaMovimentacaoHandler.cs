using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.CQRS.Movimentação.Querys;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.DTOs.Movimentações.Get.Filtros;
using Financ.Application.Mapeamento;
using Financ.Application.Services;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.ContasBancarias;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Handlers
{
    public class RetornaMovimentacaoHandler : IRequestHandler<RetornaMovimentacaoQuery, Resultado<BaseGet<RetornaMovimentacaoDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RetornaMovimentacaoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BaseGet<RetornaMovimentacaoDTO>>> Handle(RetornaMovimentacaoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Conta? conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.IdConta);
                if (conta is null)
                    return Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada"));

                ContaUsuario? contaUsuario = conta!.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);

                if (contaUsuario is null) return
                        Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Usuário não pertence a conta!"));

                contaUsuario!.ValidaSituacaoUsuarioParaConsulta();

                decimal saldoRealConcluido = await _unitOfWork.movimentacaoRepositorio
              .SomaTotalConcluidas(request.IdConta, request.Filtros.DthrMovimentacaoFinal); //Soma tudo desde o inicio das movimentacoes

                decimal saldoRealPendente = await _unitOfWork.movimentacaoRepositorio
             .SomaTotalPendentes(request.IdConta, request.Filtros.DthrMovimentacaoFinal);

                List<Movimentacao> movimentacoes = await MovimentacoesSelecionadas(request, contaUsuario);
                var fixos = await _unitOfWork.movimentacaoFixaRepositorio.BuscaMovimentacoesFixaCompleta(x => x.IdConta == request.IdConta && x.Status == StatusMovimentacaoFixa.Ativo).ToListAsync();

                if (fixos.Any())
                {
                    VirtualizaMovimentacoesFixasService virtualizaMovimentacao =
                       new VirtualizaMovimentacoesFixasService(movimentacoes.Where(m => m.Conta == contaUsuario.Conta && m.IdFixo == null), fixos, fixos.Min(x => x.DataInicio), request.Filtros.DthrMovimentacaoFinal, contaUsuario);

                    var mensal = virtualizaMovimentacao.Mensal();
                    var anual = virtualizaMovimentacao.Anual();
                    var diario = virtualizaMovimentacao.Diario();

                    saldoRealPendente += mensal.Sum(x => x.Tipo == TipoMovimentacao.Entrada ? x.Valor :
                                            x.Tipo == TipoMovimentacao.Saida ? -x.Valor : 0);
                    saldoRealPendente += anual.Sum(x => x.Tipo == TipoMovimentacao.Entrada ? x.Valor :
                                           x.Tipo == TipoMovimentacao.Saida ? -x.Valor : 0);
                    saldoRealPendente += diario.Sum(x => x.Tipo == TipoMovimentacao.Entrada ? x.Valor :
                                           x.Tipo == TipoMovimentacao.Saida ? -x.Valor : 0);
                }

                movimentacoes = movimentacoes.Where(x => x.Status != StatusMovimentacao.Excluido).ToList();

                decimal totalEntradaConcluidos = movimentacoes.Where(x => x.Tipo is TipoMovimentacao.Entrada && x.Status is StatusMovimentacao.Concluido).Sum(x => x.Valor);
                decimal totalSaidaConcluidos = movimentacoes.Where(x => x.Tipo == TipoMovimentacao.Saida && x.Status is StatusMovimentacao.Concluido).Sum(x => x.Valor);

                decimal totalEntradaPendentes = movimentacoes.Where(x => x.Tipo is TipoMovimentacao.Entrada && x.Status is StatusMovimentacao.Pendente).Sum(x => x.Valor);
                decimal totalSaidaPendentes = movimentacoes.Where(x => x.Tipo == TipoMovimentacao.Saida && x.Status is StatusMovimentacao.Pendente).Sum(x => x.Valor);

                decimal totalEntrada = totalEntradaConcluidos + totalEntradaPendentes;
                decimal totalSaida = totalSaidaConcluidos + totalSaidaPendentes;

                decimal saldoRealizado = totalEntradaConcluidos - totalSaidaConcluidos;
                decimal saldoProjetado = (totalEntradaConcluidos + totalEntradaPendentes) - (totalSaidaConcluidos + totalSaidaPendentes);




                decimal saldoRealProjetado = saldoRealConcluido + saldoRealPendente;

                if (request.Filtros.Concluido.HasValue)
                {
                    if (!request.Filtros.Concluido.Value)
                    {
                        totalEntrada = totalEntradaPendentes;
                        saldoRealizado = 0;
                    }
                    else
                    {
                        saldoRealizado = totalEntradaConcluidos - totalSaidaConcluidos;
                        saldoProjetado = 0;
                    }
                }



                GrupoMovimentacaoDTO grupoEntrada = new GrupoMovimentacaoDTO(totalEntradaConcluidos, totalEntradaPendentes, totalEntrada);
                GrupoMovimentacaoDTO grupoSaida = new GrupoMovimentacaoDTO(totalSaidaConcluidos, totalSaidaPendentes, totalSaida);

                ResumoMovimentacoesDTO resumoDTO = new ResumoMovimentacoesDTO(saldoRealConcluido, saldoRealProjetado, saldoRealizado, saldoProjetado, grupoEntrada, grupoSaida);

                return Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraSucesso(new BaseGet<RetornaMovimentacaoDTO>(MovimentacaoMapper.ParaDTO(resumoDTO, movimentacoes)));
            }
            catch (ContasUsuariosValidacao ex)
            {
                return Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
        private async Task<List<Movimentacao>> MovimentacoesSelecionadas(RetornaMovimentacaoQuery request, ContaUsuario contaUsuario)
        {

            var filtro = request.Filtros;

            var queryableMovimentacao = _unitOfWork.movimentacaoRepositorio
                .BuscaMovimentacaoComContasUsuarios();

            var queryableFixos = _unitOfWork.movimentacaoFixaRepositorio.BuscaMovimentacoesFixaCompleta(x => x.Movimentacao.Conta.Id == request.IdConta && x.Status == StatusMovimentacaoFixa.Ativo);

            bool filtraFixo = request.Filtros.RetornaFixos.HasValue && request.Filtros.RetornaFixos.Value;

            queryableMovimentacao = queryableMovimentacao.Where(x => x.IdConta == request.IdConta && x.DthrMovimentacao >= filtro.DthrMovimentacaoInicial && x.DthrMovimentacao <= filtro.DthrMovimentacaoFinal);

            if (1 == 1) //Não retorna movimentacoes excluidas
                queryableMovimentacao = queryableMovimentacao.Where(x => x.Status != StatusMovimentacao.Oculta);

            if (filtro!.Titulo is not null)
            {
                queryableMovimentacao = queryableMovimentacao.Where(x => x.Titulo.Contains(filtro.Titulo));
                queryableFixos = queryableFixos.Where((x => x.Movimentacao.Titulo.Contains(filtro.Titulo)));
            }

            if (filtro!.IdMovimentacao.HasValue == true)
            {
                queryableMovimentacao = queryableMovimentacao.Where(x => x.Id == filtro.IdMovimentacao.Value);
                filtraFixo = false;
            }

            if (filtro!.Concluido.HasValue == true)
            {
                var status = filtro.Concluido.Value
                    ? StatusMovimentacao.Concluido
                    : StatusMovimentacao.Pendente;

                queryableMovimentacao = queryableMovimentacao.Where(x => x.Status == status);

                if (status == StatusMovimentacao.Concluido)
                    filtraFixo = false;
            }

            if (filtro!.TipoMovimentacao.HasValue == true)
            {
                var tipoMovimentacao = filtro.TipoMovimentacao.Value;

                queryableMovimentacao = queryableMovimentacao.Where(x => x.Tipo == tipoMovimentacao);
                queryableFixos = queryableFixos.Where((x => x.Movimentacao.Tipo == tipoMovimentacao));
            }

            if (filtro?.IdCategoria?.Any() == true)
            {
                if (filtro.IdCategoria.Any(x => x == 0))
                {
                    queryableMovimentacao = queryableMovimentacao
                     .Where(x => !x.CategoriasMovimentacao.Any())
                     .Include(x => x.CategoriasMovimentacao)
                     .ThenInclude(mc => mc.Categoria);

                    queryableFixos = queryableFixos
                     .Where(x => !x.Movimentacao.CategoriasMovimentacao.Any())
                     .Include(x => x.Movimentacao.CategoriasMovimentacao)
                     .ThenInclude(mc => mc.Categoria);
                }
                else
                {
                    queryableMovimentacao = queryableMovimentacao
                     .Where(x => x.CategoriasMovimentacao.Any(mc => filtro!.IdCategoria.Contains(mc.IdCategoria)))
                     .Include(x => x.CategoriasMovimentacao
                     .Where(mc => filtro.IdCategoria.Contains(mc.IdCategoria)))
                     .ThenInclude(mc => mc.Categoria);

                    queryableFixos = queryableFixos
                     .Where(x => x.Movimentacao.CategoriasMovimentacao.Any(mc => filtro!.IdCategoria.Contains(mc.IdCategoria)))
                     .Include(x => x.Movimentacao.CategoriasMovimentacao
                     .Where(mc => filtro.IdCategoria.Contains(mc.IdCategoria)))
                     .ThenInclude(mc => mc.Categoria);
                }
            }

            List<Movimentacao> movimentacoes = await queryableMovimentacao.ToListAsync();

            if (filtraFixo)
            {
                List<MovimentacaoFixa> movimentacaoFixas = await queryableFixos.ToListAsync();
                List<Movimentacao> movimentacoesFixasGeradas = RetornaFixos(filtro!, movimentacoes, movimentacaoFixas, contaUsuario);
                movimentacoes.AddRange(movimentacoesFixasGeradas);
            }

            return movimentacoes.OrderByDescending(x => x.DthrMovimentacao).ToList();
        }

        private List<Movimentacao> RetornaFixos(FiltroRetornoMovimentacao filtros, IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixos, ContaUsuario contasUsuario)
        {
            VirtualizaMovimentacoesFixasService virtualizaMovimentacao =
                new VirtualizaMovimentacoesFixasService(movimentacoes, fixos, filtros.DthrMovimentacaoInicial, filtros.DthrMovimentacaoFinal, contasUsuario);

            var mensal = virtualizaMovimentacao.Mensal();
            var anual = virtualizaMovimentacao.Anual();
            var diario = virtualizaMovimentacao.Diario();

            List<Movimentacao> novaMov = new();
            novaMov.AddRange(mensal);
            novaMov.AddRange(anual);
            novaMov.AddRange(diario);
            return novaMov;
        }
    }
}
