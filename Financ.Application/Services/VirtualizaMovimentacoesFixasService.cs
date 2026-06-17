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
        private DateTime _dataInicioFiltro;
        private DateTime _dataFimFiltro;
        private ContaUsuario _contaUsuario;


        public VirtualizaMovimentacoesFixasService(IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixas, DateTime dataInicio, DateTime dataFim, ContaUsuario contaUsuario)
        {
            _movimentacoes = movimentacoes;
            _fixas = fixas;
            _dataInicioFiltro = dataInicio;
            _dataFimFiltro = dataFim;
            _contaUsuario = contaUsuario;
        }

        public List<Movimentacao> Mensal()
        {
            int diferencaMes = (_dataFimFiltro.Year - _dataInicioFiltro.Year) * 12 + (_dataFimFiltro.Month - _dataInicioFiltro.Month);
            List<Movimentacao> novaMov = new();

            var indiceProcura = _movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month))
                              .ToHashSet();


            var periodoFixos = _fixas.Where(x =>
    x.Tipo == TipoMovimentacaoFixa.Mensal &&
    x.DataInicio.Date <= _dataFimFiltro.Date &&
    x.DataFim.Date >= _dataInicioFiltro.Date)
    .ToList();

            if (!periodoFixos.Any())
                return new List<Movimentacao>();




            for (int i = 0; i <= diferencaMes; i++)
            {
                DateTime proximoDt = i > 0 ? new DateTime(_dataInicioFiltro.Year, _dataInicioFiltro.Month, 1).AddMonths(i) : _dataInicioFiltro;

                foreach (var fixo in periodoFixos)
                {
                    int diaOcorrencia = fixo.DataOcorrencia!.Value.Day;
                    int ultimoDiaMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);

                    int diaValido = Math.Min(diaOcorrencia, ultimoDiaMes);

                    DateTime dthrMovimentacao = new(
                        proximoDt.Year,
                        proximoDt.Month,
                        diaValido);

                    if (dthrMovimentacao.Date >= fixo.DataInicio.Date &&
                        dthrMovimentacao.Date <= fixo.DataFim.Date &&
                        dthrMovimentacao.Date <= _dataFimFiltro.Date)
                    {


                        if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, proximoDt.Month)))
                        {
                            var mov = new Movimentacao(fixo.Movimentacao.Tipo, _contaUsuario, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo, true);

                            fixo.Movimentacao.CategoriasMovimentacao.ToList().ForEach(x => mov.AdicionarCategoria(x));


                            novaMov.Add(mov);

                            indiceProcura.Add((fixo.Id, proximoDt.Year, proximoDt.Month));
                        }
                    }
                }
            }

            return novaMov;
        }
        public List<Movimentacao> Anual()
        {

            int diferencaAno = (int)Math.Ceiling(((_dataFimFiltro.Year - _dataInicioFiltro.Year) * 12 + (_dataFimFiltro.Month - _dataInicioFiltro.Month)) / 12.0);

            List<Movimentacao> novaMov = new();

            var indiceProcura = _movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month, m.DthrMovimentacao.Day))
                              .ToHashSet();

            var periodoFixos = _fixas.Where(x =>
x.Tipo == TipoMovimentacaoFixa.Anual &&
x.DataInicio.Date <= _dataFimFiltro.Date &&
x.DataFim.Date >= _dataInicioFiltro.Date)
.ToList();


            for (int i = 0; i <= diferencaAno; i++)
            {
                DateTime proximoDt = i > 0 ? new DateTime(_dataInicioFiltro.Year, 1, 1).AddYears(i) : _dataInicioFiltro;

                foreach (var fixo in periodoFixos.Where(f =>
                         new DateTime(proximoDt.Year, f.DataOcorrencia!.Value.Month, f.DataOcorrencia!.Value.Day).Date >= f.DataInicio.Date &&
                         new DateTime(proximoDt.Year, f.DataOcorrencia!.Value.Month, f.DataOcorrencia!.Value.Day).Date <= f.DataFim.Date &&
                          new DateTime(proximoDt.Year, f.DataOcorrencia!.Value.Month, f.DataOcorrencia!.Value.Day).Date <= _dataFimFiltro.Date))
                {
                    int diasMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);

                    diasMes = fixo.DataOcorrencia!.Value.Day > diasMes ? diasMes : fixo.DataOcorrencia!.Value.Day;

                    DateTime dthrMovimentacao = new DateTime(proximoDt.Year, fixo.DataOcorrencia!.Value.Month, diasMes, fixo.DataOcorrencia!.Value.Hour, fixo.DataOcorrencia!.Value.Minute, fixo.DataOcorrencia!.Value.Second);

                    if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, fixo.DataOcorrencia!.Value.Month, fixo.DataOcorrencia!.Value.Day)))
                    {
                        var mov = new Movimentacao(fixo.Movimentacao.Tipo, _contaUsuario, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo, true);

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
            int diferencaMes = (_dataFimFiltro.Year - _dataInicioFiltro.Year) * 12 + (_dataFimFiltro.Month - _dataInicioFiltro.Month);
            List<Movimentacao> novaMov = new();

            var indiceProcura = _movimentacoes
                              .Select(m => (m.IdFixo, m.DthrMovimentacao.Year, m.DthrMovimentacao.Month, m.DthrMovimentacao.Day))
                              .ToHashSet();

            var periodoFixos = _fixas.Where(x =>
x.Tipo == TipoMovimentacaoFixa.Diaria &&
x.DataInicio.Date <= _dataFimFiltro.Date &&
x.DataFim.Date >= _dataInicioFiltro.Date)
.ToList();


            for (int i = 0; i <= diferencaMes; i++)
            {
                DateTime proximoDt =  _dataInicioFiltro.AddMonths(i);

                int diasMes = DateTime.DaysInMonth(proximoDt.Year, proximoDt.Month);


                for (int j = 1; j <= diasMes; j++)
                {
                    int diaSemana = (int)new DateTime(proximoDt.Year, proximoDt.Month, j).DayOfWeek;
                    DateTime dthrMovimentacao = new DateTime(proximoDt.Year, proximoDt.Month, j, 12, 0, 0);

                    foreach (var fixo in periodoFixos.Where(f =>
                              dthrMovimentacao.Date >= f.DataInicio.Date &&
                              dthrMovimentacao.Date <= f.DataFim.Date &&
                              dthrMovimentacao.Date <= _dataFimFiltro.Date))
                    {

                        if (fixo.DiasFixosDiarios!.Any(x => x.DiaSemana == diaSemana))
                        {
                            if (!indiceProcura.Contains((fixo.Id, proximoDt.Year, proximoDt.Month, j)))
                            {

                                var mov = new Movimentacao(fixo.Movimentacao.Tipo, _contaUsuario, fixo.Movimentacao.Valor, fixo.Movimentacao.Titulo, fixo.Movimentacao.Observacao, dthrMovimentacao, null, false, fixo, true);

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
