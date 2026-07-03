using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.Movimentações.Fixas;
using Financ.Domain.Validacoes.Movimentações.Fixas.Mensagens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.Movimentações.Fixas
{
    public class MovimentacaoFixa
    {
    //    public int Id { get; private set; }
    //    public int IdMovimentacao { get; private set; }
    //    public int IdConta { get; private set; }
    //    public Conta Conta { get; set; }
    //    public ETipoMovimentacaoFixa Tipo { get; private set; }
    //    public EStatusMovimentacaoFixa Status { get; private set; }
    //    public DateTime DataInicio
    //    { get; private set; }
    //    public DateTime DataFim { get; private set; }
    //    public DateTime? DataOcorrencia { get; private set; }
    //    public DateTime Dthr { get; private set; }
    //    public Movimentacao Movimentacao { get; private set; }
    //    public ICollection<Movimentacao> Movimentacoes { get; private set; } = new List<Movimentacao>();
    //    public ICollection<MovimentacaoFixaDiaria>? DiasFixosDiarios { get; private set; } = new List<MovimentacaoFixaDiaria>();
    //    public MovimentacaoFixa() { }

    //    public MovimentacaoFixa(ETipoMovimentacaoFixa tipo, DateTime dataInicio, DateTime dataFim, DateTime dataOcorrencia, Movimentacao movimentacao)
    //    {
    //        ValidaDatas(dataInicio, dataFim);
    //        DataInicio = dataInicio;
    //        DataFim = dataFim;
    //        DataOcorrencia = dataOcorrencia;
    //        Dthr = DateTime.UtcNow;
    //        ValidaTipoFixo(tipo);
    //        MovimentacaoFixaValidacao.Verifica(tipo == ETipoMovimentacaoFixa.Diaria, MensagemMovimentacaoFixa.TIPO_DIARIO_NAO_PODE_SER_CRIA_COM_DATA_OCORRENCIA);
    //        Conta = movimentacao.Conta;
    //        IdConta = movimentacao.IdConta;
    //        Tipo = tipo;
    //        movimentacao.CriaMovimentacaoFixa(this);
    //        ValidaStatusMovimentacaoBase(movimentacao.Status);
    //        Status = EStatusMovimentacaoFixa.Ativo;
    //        IdMovimentacao = movimentacao.Id;
    //        Movimentacao = movimentacao;
    //    }
    //    public MovimentacaoFixa(DateTime dataInicio, DateTime dataFim, int[] ocorrenciaDiaria, Movimentacao movimentacao)
    //    {
    //        MovimentacaoFixaValidacao.Verifica(!ocorrenciaDiaria.Any(), MensagemMovimentacaoFixa.MOVIMENTACAO_DIARIA_NAO_INFORMADA);

    //        ValidaDatas(dataInicio, dataFim);
    //        DataInicio = dataInicio;
    //        DataFim = dataFim;
    //        Dthr = DateTime.UtcNow;
    //        Tipo = ETipoMovimentacaoFixa.Diaria;
    //        movimentacao.CriaMovimentacaoFixa(this);
    //        Conta = movimentacao.Conta;
    //        IdConta = movimentacao.IdConta;
    //        ValidaStatusMovimentacaoBase(movimentacao.Status);
    //        Status = EStatusMovimentacaoFixa.Ativo;
    //        IdMovimentacao = movimentacao.Id;
    //        Movimentacao = movimentacao;


    //        ocorrenciaDiaria.ToList().ForEach(ms => DiasFixosDiarios.Add(new MovimentacaoFixaDiaria(this, ms)));

    //    }
    //    public Movimentacao MaterializaMovimentacao(DateTime darMovimentacao, ContaUsuario? contaUsuario)
    //    {
    //        DateTime dataMovimentacao = darMovimentacao;
    //        MovimentacaoFixaValidacao.Verifica(!(dataMovimentacao.Date >= DataInicio.Date && dataMovimentacao.Date <= DataFim.Date), "A data de movimentação está fora do perído da movimentação fixa");

    //        if (Tipo is ETipoMovimentacaoFixa.Diaria)
    //        {
    //            MovimentacaoFixaValidacao.Verifica(Movimentacoes.Any(m => m.DthrMovimentacao.Date == dataMovimentacao.Date), "Movimentação fixa já está meterializada para este período");

    //            MovimentacaoFixaValidacao.Verifica(!(DiasFixosDiarios!.Any(x => x.DiaSemana == (int)dataMovimentacao.DayOfWeek)), "Data de ocorrencia errada");
    //        }

    //        if (Tipo is ETipoMovimentacaoFixa.Mensal)
    //        {
    //            MovimentacaoFixaValidacao.Verifica(Movimentacoes.Any(m => m.DthrMovimentacao.Month == dataMovimentacao.Month && m.DthrMovimentacao.Year == dataMovimentacao.Year), "Movimentação fixa já está meterializada para este período");

    //            MovimentacaoFixaValidacao.Verifica(!(dataMovimentacao.Day == DataOcorrencia!.Value.Day), "Data de ocorrencia errada");
    //        }
    //        if (Tipo is ETipoMovimentacaoFixa.Anual)
    //        {
    //            MovimentacaoFixaValidacao.Verifica(Movimentacoes.Any(m => m.DthrMovimentacao.Year == dataMovimentacao.Year), "Movimentação fixa já está meterializada para este período");

    //            MovimentacaoFixaValidacao.Verifica(!(dataMovimentacao.Month == DataOcorrencia!.Value.Month), "Data de ocorrencia errada");
    //        }

    //        Movimentacao movimentacaoMaterializada = new Movimentacao(Movimentacao.Tipo, contaUsuario, Movimentacao.Valor, Movimentacao.Titulo, Movimentacao.Observacao, darMovimentacao, Movimentacao.DthrConclusao, false, this);
    //        Movimentacao.CategoriasMovimentacao.ToList().ForEach(x =>
    //movimentacaoMaterializada.AdicionarCategoria(new MovimentacaoCategoria(movimentacaoMaterializada, x.Categoria)));
    //        return movimentacaoMaterializada;
    //    }
    //    public void AlteraMovimentacaoFixa(ContaUsuario? contaUsuario, ETipoMovimentacaoFixa? tipo, EStatusMovimentacaoFixa? status, DateTime? dataInicio, DateTime? dataFim, DateTime? dataOcorrencia)
    //    {
    //        MovimentacaoFixaValidacao.Verifica(contaUsuario is null, "Usuário não pertence a esta conta.");

    //        if (status.HasValue)
    //        {
    //            ValidaStatus(status.Value);
    //            Status = status.Value;
    //        }

    //        if (dataInicio.HasValue || dataFim.HasValue)
    //        {
    //            ValidaDatas(dataInicio.HasValue ? dataInicio.Value : DataInicio, dataFim.HasValue ? dataFim.Value : DataFim);

    //            DataInicio = dataInicio.HasValue ? dataInicio.Value : DataInicio;
    //            DataFim = dataFim.HasValue ? dataFim.Value : DataFim;
    //        }

    //        if (dataOcorrencia.HasValue)
    //            DataOcorrencia = dataOcorrencia!.Value;

    //        if (tipo.HasValue)
    //            Tipo = tipo.Value;
    //    }

    //    public void AlteraMovimentacaoFixaDiaria(ContaUsuario? contaUsuario, EStatusMovimentacaoFixa? status, DateTime? dataInicio, DateTime? dataFim, int[]? ocorrenciaDiaria)
    //    {
    //        AlteraMovimentacaoFixa(contaUsuario, null, status, dataInicio, dataFim, null);
    //        if (ocorrenciaDiaria is not null)
    //        {
    //            MovimentacaoFixaValidacao.Verifica(!ocorrenciaDiaria.Any(), MensagemMovimentacaoFixa.MOVIMENTACAO_DIARIA_NAO_INFORMADA);
    //            DiasFixosDiarios!.Clear();
    //            ocorrenciaDiaria.ToList().ForEach(ms => DiasFixosDiarios.Add(new MovimentacaoFixaDiaria(this, ms)));
    //        }
    //    }
    //    private void ValidaDatas(DateTime dataInicio, DateTime dataFim)
    //    {
    //        MovimentacaoFixaValidacao.Verifica(dataInicio.Date >= dataFim.Date, MensagemMovimentacaoFixa.DATAS_INVALIDAS);
    //    }
    //    private void ValidaTipoFixo(ETipoMovimentacaoFixa tipo)
    //    {
    //        MovimentacaoFixaValidacao.Verifica(!Enum.IsDefined(typeof(ETipoMovimentacaoFixa), tipo), MensagemMovimentacaoFixa.TIPO_INVALIDO);
    //    }
    //    private void ValidaStatusMovimentacaoBase(EStatusMovimentacao status)
    //    {
    //        MovimentacaoFixaValidacao.Verifica(status != EStatusMovimentacao.Oculta, MensagemMovimentacaoFixa.MOVIMENTACAO_NAO_ESTA_OCULTA);
    //    }
    //    private void ValidaStatus(EStatusMovimentacaoFixa status)
    //    {
    //        MovimentacaoFixaValidacao.Verifica(!Enum.IsDefined(typeof(EStatusMovimentacaoFixa), status), "Status de movimentaçao fixa inválida.");
    //    }
    }
}
