using Financ.Application.DTOs.Movimentações.Get.Filtros;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.Movimentações.Fixas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services
{
    public class VirtualizaMovimentacoesFixasService
    {
        private IEnumerable<Movimentacao> _movimentacoes;
        private IEnumerable<MovimentacaoFixa> _fixas;
        private DateOnly _dataInicio;
        private DateOnly _dataFim;
        private ContaUsuario _contaUsuario;

        public VirtualizaMovimentacoesFixasService(IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixas, DateTime dataInicio, DateTime dataFim,ContaUsuario contaUsuario)
        {
            _movimentacoes = movimentacoes;
            _fixas = fixas;
            _dataInicio = DateOnly.FromDateTime(dataInicio);
            _dataFim = DateOnly.FromDateTime(dataFim);
            _contaUsuario = contaUsuario;
        }

        public VirtualizaMovimentacoesFixasService(IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixas, DateOnly dataInicio, DateOnly dataFim, ContaUsuario contaUsuario)
        {
            _movimentacoes = movimentacoes;
            _fixas = fixas;
            _dataInicio = dataInicio;
            _dataFim = dataFim;
            _contaUsuario = contaUsuario;
        }

        public List<Movimentacao> Mensal()
        {
             int diferencaMes = (_dataFim.Year - _dataInicio.Year) * 12 + (_dataFim.Month - _dataInicio.Month);
            List<Movimentacao> novaMov = new();

            var indiceProcura = _movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month))
                              .ToHashSet();

            var periodoFixos = _fixas.Where(x => x.Tipo == TipoMovimentacaoFixa.Mensal && (_dataInicio >= x.DataInicio || _dataInicio <= x.DataFim)).ToList();

            if (!periodoFixos.Any())
                return new List<Movimentacao>();

            for (int i = 0; i <= diferencaMes; i++)
            {
                DateOnly proximoDt = i > 0 ? new DateOnly(_dataInicio.Year, _dataInicio.Month, 1).AddMonths(i) : _dataInicio;
                foreach (var fixo in periodoFixos.Where(
                    x => proximoDt >= x.DataInicio &&
                    proximoDt <= x.DataFim).ToList())
                {

                    if (i == 0 && (proximoDt.Day > fixo.DataOcorrencia!.Value.Day))
                    {
                        continue;
                    }

                    if (i == diferencaMes && fixo.DataOcorrencia!.Value.Day > _dataFim.Day)
                    {
                        continue;
                    }

                    if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, proximoDt.Month)))
                    {

                        int diasMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);

                        diasMes = fixo.DataOcorrencia!.Value.Day > diasMes ? diasMes : fixo.DataOcorrencia!.Value.Day;

                        DateTime dthrMovimentacao = new DateTime(proximoDt.Year, proximoDt.Month, diasMes, fixo.DataOcorrencia!.Value.Hour, fixo.DataOcorrencia!.Value.Minute, fixo.DataOcorrencia!.Value.Second);

                        var mov = new Movimentacao(fixo.Movimentacao.Tipo, _contaUsuario, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo);

                        fixo.Movimentacao.CategoriasMovimentacao.ToList().ForEach(x => mov.AdicionarCategoria(x));


                        novaMov.Add(mov);

                        indiceProcura.Add((fixo.Id, proximoDt.Year, proximoDt.Month));
                    }
                }
            }

            return novaMov;
        }
        public List<Movimentacao> Anual()
        {

            int diferencaAno = (int)Math.Ceiling(((_dataFim.Year - _dataInicio.Year) * 12 + (_dataFim.Month - _dataInicio.Month)) / 12.0);

            List<Movimentacao> novaMov = new();

            var indiceProcura = _movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month, m.DthrMovimentacao.Day))
                              .ToHashSet();

            var periodoFixos = _fixas.Where(x => x.Tipo == TipoMovimentacaoFixa.Anual && (_dataInicio >= x.DataInicio || _dataInicio <= x.DataFim)).ToList();
            for (int i = 0; i <= diferencaAno; i++)
            {
                DateOnly proximoDt = i > 0 ? new DateOnly(_dataInicio.Year, 1, 1).AddYears(i) : _dataInicio;

                foreach (var fixo in periodoFixos.Where(x => proximoDt.Year >= x.DataInicio.Year &&
                                                             proximoDt.Year <= x.DataFim.Year).ToList())
                {
                    int diasMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);

                    diasMes = fixo.DataOcorrencia!.Value.Day > diasMes ? diasMes : fixo.DataOcorrencia!.Value.Day;

                    DateTime dthrMovimentacao = new DateTime(proximoDt.Year, fixo.DataOcorrencia!.Value.Month, diasMes, fixo.DataOcorrencia!.Value.Hour, fixo.DataOcorrencia!.Value.Minute, fixo.DataOcorrencia!.Value.Second);

                    if (!(DateOnly.FromDateTime(dthrMovimentacao) >= _dataInicio && DateOnly.FromDateTime(dthrMovimentacao) <= _dataFim))
                        continue;

                    if (DateOnly.FromDateTime(dthrMovimentacao) >= fixo.DataFim)
                        break;

                    if ((i == 0 && (fixo.DataOcorrencia!.Value.Day < _dataInicio.Day || fixo.DataOcorrencia!.Value.Month < _dataInicio.Month)))
                    {
                        continue;
                    }

                    //if ((i == diferencaAno && (fixo.DataOcorrencia!.Value.Day > _dataFim.Day || fixo.DataOcorrencia!.Value.Month > _dataFim.Month)))
                    //{
                    //    continue;
                    //}

                    if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, fixo.DataOcorrencia!.Value.Month, fixo.DataOcorrencia!.Value.Day)))
                    {
                        var mov = new Movimentacao(fixo.Movimentacao.Tipo, _contaUsuario, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo);

                        fixo.Movimentacao.CategoriasMovimentacao.ToList().ForEach(x => mov.AdicionarCategoria(x));

                        novaMov.Add(mov);

                        indiceProcura.Add((fixo.Id, proximoDt.Year, fixo.DataOcorrencia!.Value.Month, fixo.DataOcorrencia!.Value.Day));
                    }
                }
            }

            return novaMov;
        }
        public List<Movimentacao> Diario()
        {
            int diferencaMes = (_dataFim.Year - _dataInicio.Year) * 12 + (_dataFim.Month - _dataInicio.Month);
            List<Movimentacao> novaMov = new();

            var indiceProcura = _movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month, m.DthrMovimentacao.Day))
                              .ToHashSet();

            var periodoFixos = _fixas.Where(x => x.Tipo == TipoMovimentacaoFixa.Diaria && (_dataInicio >= x.DataInicio || _dataInicio <= x.DataFim)).ToList();

            for (int i = 0; i <= diferencaMes; i++)
            {
                DateOnly proximoDt = _dataInicio.AddMonths(i);

                foreach (var fixo in periodoFixos.Where(x => proximoDt >= x.DataInicio && proximoDt <= x.DataFim).ToList())
                {
                    int diasMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);

                    for (int j = 1; j <= diasMes; j++)
                    {
                        int diaSemana = (int)new DateTime(proximoDt.Year, proximoDt.Month, j).DayOfWeek;
                        DateTime dthrMovimentacao = new DateTime(proximoDt.Year, proximoDt.Month, j, 12, 0, 0);


                        if ((i == diferencaMes && DateOnly.FromDateTime(dthrMovimentacao) > _dataFim || DateOnly.FromDateTime(dthrMovimentacao) > fixo.DataFim))
                        {
                            break;
                        }


                        if (fixo.DiasFixosDiarios!.Any(x => x.DiaSemana == diaSemana))
                        {
                            if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, proximoDt.Month, j)))
                            {

                                var mov = new Movimentacao(fixo.Movimentacao.Tipo, _contaUsuario, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo);

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
