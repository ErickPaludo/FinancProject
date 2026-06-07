using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.CQRS.Movimentação.Querys;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.DTOs.Movimentações.Get.Filtros;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.ContasBancarias;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
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

                List<Movimentacao> movimentacoes = await MovimentacoesSelecionadas(request);

                decimal totalEntradaConcluidos = movimentacoes.Where(x => x.Tipo is TipoMovimentacao.Entrada && x.Status is StatusMovimentacao.Concluido).Sum(x => x.Valor);
                decimal totalSaidaConcluidos = movimentacoes.Where(x => x.Tipo == TipoMovimentacao.Saida && x.Status is StatusMovimentacao.Concluido).Sum(x => x.Valor);

                decimal totalEntradaPendentes = movimentacoes.Where(x => x.Tipo is TipoMovimentacao.Entrada && x.Status is StatusMovimentacao.Pendente).Sum(x => x.Valor);
                decimal totalSaidaPendentes = movimentacoes.Where(x => x.Tipo == TipoMovimentacao.Saida && x.Status is StatusMovimentacao.Pendente).Sum(x => x.Valor);

                decimal totalEntrada = totalEntradaConcluidos + totalEntradaPendentes;
                decimal totalSaida = totalSaidaConcluidos + totalSaidaPendentes;

                decimal saldoRealizado = totalEntradaConcluidos - totalSaidaConcluidos;
                decimal saldoProjetado = (totalEntradaConcluidos + totalEntradaPendentes) - (totalSaidaConcluidos + totalSaidaPendentes);

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

                ResumoMovimentacoesDTO resumoDTO = new ResumoMovimentacoesDTO(saldoRealizado, saldoProjetado, grupoEntrada, grupoSaida);

                return Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraSucesso(new BaseGet<RetornaMovimentacaoDTO>(MovimentacaoMapper.ParaDTO(resumoDTO, movimentacoes)));
            }
            catch (ContasUsuariosValidacao ex)
            {
                return Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
        private async Task<List<Movimentacao>> MovimentacoesSelecionadas(RetornaMovimentacaoQuery request)
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
                List<Movimentacao> movimentacoesFixasGeradas = RetornaFixos(filtro!, movimentacoes, movimentacaoFixas);
                movimentacoes.AddRange(movimentacoesFixasGeradas);
            }

            return movimentacoes.Where(x => x.Status != StatusMovimentacao.Excluido).OrderByDescending(x => x.DthrMovimentacao).ToList();
        }

        private List<Movimentacao> RetornaFixos(FiltroRetornoMovimentacao filtros, IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixos)
        {
            var mensal = Mensal(filtros, movimentacoes, fixos);
            var anual = Anual(filtros, movimentacoes, fixos);
            var diario = Diario(filtros, movimentacoes, fixos);
            List<Movimentacao> novaMov = new();
            novaMov.AddRange(mensal);
            novaMov.AddRange(anual);
            novaMov.AddRange(diario);
            return novaMov;
        }
        private List<Movimentacao> Mensal(FiltroRetornoMovimentacao filtros, IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixos)
        {
            DateOnly inicio = DateOnly.FromDateTime(filtros.DthrMovimentacaoInicial);
            DateOnly fim = DateOnly.FromDateTime(filtros.DthrMovimentacaoFinal);

            int diferencaMes = (fim.Year - inicio.Year) * 12 + (fim.Month - inicio.Month);
            List<Movimentacao> novaMov = new();

            var indiceProcura = movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month))
                              .ToHashSet();

            var periodoFixos = fixos.Where(x => x.Tipo == TipoMovimentacaoFixa.Mensal && (inicio >= x.DataInicio || inicio <= x.DataFim)).ToList();

            if (!periodoFixos.Any())
                return new List<Movimentacao>();

            for (int i = 0; i <= diferencaMes; i++)
            {
                DateOnly proximoDt = i > 0 ? new DateOnly(inicio.Year, inicio.Month, 1).AddMonths(i) : inicio;
                foreach (var fixo in periodoFixos.Where(
                    x => proximoDt >= x.DataInicio &&
                    proximoDt <= x.DataFim).ToList())
                {

                    if (i == 0 && (proximoDt.Day > fixo.DataOcorrencia!.Value.Day))
                    {
                        continue;
                    }

                    if (i == diferencaMes && fixo.DataOcorrencia!.Value.Day > fim.Day)
                    {
                        continue;
                    }

                    if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, proximoDt.Month)))
                    {

                        int diasMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);

                        diasMes = fixo.DataOcorrencia!.Value.Day > diasMes ? diasMes : fixo.DataOcorrencia!.Value.Day;

                        DateTime dthrMovimentacao = new DateTime(proximoDt.Year, proximoDt.Month, diasMes, fixo.DataOcorrencia!.Value.Hour, fixo.DataOcorrencia!.Value.Minute, fixo.DataOcorrencia!.Value.Second);

                        var mov = new Movimentacao(fixo.Movimentacao.Tipo, fixo.Movimentacao.ContaUsuarioCriador, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo);

                        fixo.Movimentacao.CategoriasMovimentacao.ToList().ForEach(x => mov.AdicionarCategoria(x));


                        novaMov.Add(mov);

                        indiceProcura.Add((fixo.Id, proximoDt.Year, proximoDt.Month));
                    }
                }
            }

            return novaMov;
        }
        private List<Movimentacao> Anual(FiltroRetornoMovimentacao filtros, IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixos)
        {
            DateOnly inicio = DateOnly.FromDateTime(filtros.DthrMovimentacaoInicial);
            DateOnly fim = DateOnly.FromDateTime(filtros.DthrMovimentacaoFinal);

            int diferencaAno = (int)Math.Ceiling(((fim.Year - inicio.Year) * 12 + (fim.Month - inicio.Month)) / 12.0);

            List<Movimentacao> novaMov = new();

            var indiceProcura = movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month, m.DthrMovimentacao.Day))
                              .ToHashSet();

            var periodoFixos = fixos.Where(x => x.Tipo == TipoMovimentacaoFixa.Anual && (inicio >= x.DataInicio || inicio <= x.DataFim)).ToList();

            for (int i = 0; i <= diferencaAno; i++)
            {
                DateOnly proximoDt = i > 0 ? new DateOnly(inicio.Year, 1, 1).AddYears(i) : inicio;

                foreach (var fixo in periodoFixos.Where(x => proximoDt.Year >= x.DataInicio.Year &&
                                                             proximoDt.Year <= x.DataFim.Year).ToList())
                {
                    int diasMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);

                    diasMes = fixo.DataOcorrencia!.Value.Day > diasMes ? diasMes : fixo.DataOcorrencia!.Value.Day;

                    DateTime dthrMovimentacao = new DateTime(proximoDt.Year, fixo.DataOcorrencia!.Value.Month, diasMes, fixo.DataOcorrencia!.Value.Hour, fixo.DataOcorrencia!.Value.Minute, fixo.DataOcorrencia!.Value.Second);

                    if (!(DateOnly.FromDateTime(dthrMovimentacao) >= inicio && DateOnly.FromDateTime(dthrMovimentacao) <= fim))
                        continue;

                    if (DateOnly.FromDateTime(dthrMovimentacao) >= fixo.DataFim)
                        break;

                    if ((i == 0 && (fixo.DataOcorrencia!.Value.Day < inicio.Day || fixo.DataOcorrencia!.Value.Month < inicio.Month)))
                    {
                        continue;
                    }

                    //if ((i == diferencaAno && (fixo.DataOcorrencia!.Value.Day > fim.Day || fixo.DataOcorrencia!.Value.Month > fim.Month)))
                    //{
                    //    continue;
                    //}

                    if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, fixo.DataOcorrencia!.Value.Month, fixo.DataOcorrencia!.Value.Day)))
                    {
                        var mov = new Movimentacao(fixo.Movimentacao.Tipo, fixo.Movimentacao.ContaUsuarioCriador, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo);

                        fixo.Movimentacao.CategoriasMovimentacao.ToList().ForEach(x => mov.AdicionarCategoria(x));

                        novaMov.Add(mov);

                        indiceProcura.Add((fixo.Id, proximoDt.Year, fixo.DataOcorrencia!.Value.Month, fixo.DataOcorrencia!.Value.Day));
                    }
                }
            }

            return novaMov;
        }
        private List<Movimentacao> Diario(FiltroRetornoMovimentacao filtros, IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixos)
        {
            DateOnly inicio = DateOnly.FromDateTime(filtros.DthrMovimentacaoInicial);
            DateOnly fim = DateOnly.FromDateTime(filtros.DthrMovimentacaoFinal);

            int diferencaMes = (fim.Year - inicio.Year) * 12 + (fim.Month - inicio.Month);
            List<Movimentacao> novaMov = new();

            var indiceProcura = movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month, m.DthrMovimentacao.Day))
                              .ToHashSet();

            var periodoFixos = fixos.Where(x => x.Tipo == TipoMovimentacaoFixa.Diaria && (inicio >= x.DataInicio || inicio <= x.DataFim)).ToList();

            for (int i = 0; i <= diferencaMes; i++)
            {
                DateOnly proximoDt = inicio.AddMonths(i);

                foreach (var fixo in periodoFixos.Where(x => proximoDt >= x.DataInicio && proximoDt <= x.DataFim).ToList())
                {
                    int diasMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);

                    for (int j = 1; j <= diasMes; j++)
                    {
                        int diaSemana = (int)new DateTime(proximoDt.Year, proximoDt.Month, j).DayOfWeek;
                        DateTime dthrMovimentacao = new DateTime(proximoDt.Year, proximoDt.Month, j, 12, 0, 0);


                        if ((i == diferencaMes && DateOnly.FromDateTime(dthrMovimentacao) > fim || DateOnly.FromDateTime(dthrMovimentacao) > fixo.DataFim))
                        {
                            break;
                        }


                        if (fixo.DiasFixosDiarios!.Any(x => x.DiaSemana == diaSemana))
                        {
                            if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, proximoDt.Month, j)))
                            {

                                var mov = new Movimentacao(fixo.Movimentacao.Tipo, fixo.Movimentacao.ContaUsuarioCriador, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo);

                                fixo.Movimentacao.CategoriasMovimentacao.ToList().ForEach(x => mov.AdicionarCategoria(x));

                                novaMov.Add(mov);

                                indiceProcura.Add((fixo.Id, proximoDt.Year, proximoDt.Month, j));
                            }
                        }
                    }

                }
            }

            return novaMov;
        }
    }
}
