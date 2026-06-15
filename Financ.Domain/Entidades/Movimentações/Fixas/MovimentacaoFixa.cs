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
        public int Id { get; private set; }
        public int IdMovimentacao { get; private set; }
        public int IdConta { get; private set; }
        public Conta Conta { get; set; }
        public TipoMovimentacaoFixa Tipo { get; private set; }
        public StatusMovimentacaoFixa Status { get; private set; }
        public DateOnly DataInicio
        { get; private set; }
        public DateOnly DataFim { get; private set; }
        public DateTime? DataOcorrencia { get; private set; }
        public DateTime Dthr { get; private set; }
        public Movimentacao Movimentacao { get; private set; }
        public ICollection<Movimentacao> Movimentacoes { get; private set; } = new List<Movimentacao>();
        public ICollection<MovimentacaoFixaDiaria>? DiasFixosDiarios { get; private set; } = new List<MovimentacaoFixaDiaria>();
        public MovimentacaoFixa() { }

        public MovimentacaoFixa(TipoMovimentacaoFixa tipo, DateOnly dataInicio, DateOnly dataFim, DateTime dataOcorrencia, Movimentacao movimentacao)
        {
            ValidaDatas(dataInicio, dataFim);
            DataInicio = dataInicio;
            DataFim = dataFim;
            DataOcorrencia = dataOcorrencia;
            Dthr = DateTime.UtcNow;
            ValidaTipoFixo(tipo);
            MovimentacaoFixaValidacao.Verifica(tipo == TipoMovimentacaoFixa.Diaria, MensagemMovimentacaoFixa.TIPO_DIARIO_NAO_PODE_SER_CRIA_COM_DATA_OCORRENCIA);
            Conta = movimentacao.Conta;
            IdConta = movimentacao.IdConta;
            Tipo = tipo;
            movimentacao.CriaMovimentacaoFixa(this);
            ValidaStatusMovimentacaoBase(movimentacao.Status);
            Status = StatusMovimentacaoFixa.Ativo;
            IdMovimentacao = movimentacao.Id;
            Movimentacao = movimentacao;
        }
        public MovimentacaoFixa(DateOnly dataInicio, DateOnly dataFim, int[] ocorrenciaDiaria, Movimentacao movimentacao)
        {
            MovimentacaoFixaValidacao.Verifica(!ocorrenciaDiaria.Any(), MensagemMovimentacaoFixa.MOVIMENTACAO_DIARIA_NAO_INFORMADA);

            ValidaDatas(dataInicio, dataFim);
            DataInicio = dataInicio;
            DataFim = dataFim;
            Dthr = DateTime.UtcNow;
            Tipo = TipoMovimentacaoFixa.Diaria;
            movimentacao.CriaMovimentacaoFixa(this);
            Conta = movimentacao.Conta;
            IdConta = movimentacao.IdConta;
            ValidaStatusMovimentacaoBase(movimentacao.Status);
            Status = StatusMovimentacaoFixa.Ativo;
            IdMovimentacao = movimentacao.Id;
            Movimentacao = movimentacao;


            ocorrenciaDiaria.ToList().ForEach(ms => DiasFixosDiarios.Add(new MovimentacaoFixaDiaria(this, ms)));

        }
        public Movimentacao MaterializaMovimentacao(DateTime darMovimentacao, ContaUsuario? contaUsuario)
        {
            DateOnly dataMovimentacao = DateOnly.FromDateTime(darMovimentacao);
            MovimentacaoFixaValidacao.Verifica(!(dataMovimentacao >= DataInicio && dataMovimentacao <= DataFim), "A data de movimentação está fora do perído da movimentação fixa");

            if (Tipo is TipoMovimentacaoFixa.Diaria)
            {
                MovimentacaoFixaValidacao.Verifica(Movimentacoes.Any(m => DateOnly.FromDateTime(m.DthrMovimentacao) == dataMovimentacao), "Movimentação fixa já está meterializada para este período");

                MovimentacaoFixaValidacao.Verifica(!(DiasFixosDiarios!.Any(x => x.DiaSemana == (int)dataMovimentacao.DayOfWeek)), "Data de ocorrencia errada");
            }

            if (Tipo is TipoMovimentacaoFixa.Mensal)
            {
                MovimentacaoFixaValidacao.Verifica(Movimentacoes.Any(m => m.DthrMovimentacao.Month == dataMovimentacao.Month && m.DthrMovimentacao.Year == dataMovimentacao.Year), "Movimentação fixa já está meterializada para este período");

                MovimentacaoFixaValidacao.Verifica(!(dataMovimentacao.Day == DataOcorrencia!.Value.Day), "Data de ocorrencia errada");
            }
            if (Tipo is TipoMovimentacaoFixa.Anual)
            {
                MovimentacaoFixaValidacao.Verifica(Movimentacoes.Any(m => m.DthrMovimentacao.Year == dataMovimentacao.Year), "Movimentação fixa já está meterializada para este período");

                MovimentacaoFixaValidacao.Verifica(!(dataMovimentacao.Month == DataOcorrencia!.Value.Month), "Data de ocorrencia errada");
            }

            Movimentacao movimentacaoMaterializada = new Movimentacao(Movimentacao.Tipo, contaUsuario, Movimentacao.Valor, Movimentacao.Titulo, Movimentacao.Observacao, darMovimentacao, Movimentacao.DthrConclusao, false, this);
            Movimentacao.CategoriasMovimentacao.ToList().ForEach(x => movimentacaoMaterializada.AdicionarCategoria(x));

            return movimentacaoMaterializada;
        }
        public void AlteraMovimentacaoFixa(ContaUsuario? contaUsuario, TipoMovimentacaoFixa? tipo,StatusMovimentacaoFixa? status, DateOnly? dataInicio, DateOnly? dataFim, DateTime? dataOcorrencia)
        {
            MovimentacaoFixaValidacao.Verifica(contaUsuario is null, "Usuário não pertence a esta conta.");
            MovimentacaoFixaValidacao.Verifica(contaUsuario!.Status != StatusContasUsuario.Ativo, "Usuário inativo.");
            MovimentacaoFixaValidacao.Verifica(contaUsuario.Acesso == TiposAcessos.Visualizador, "Usuário não possui permissão para esta ação.");

            if(status.HasValue)
            {
                ValidaStatus(status.Value);
                Status = status.Value;
            }

            if (dataInicio.HasValue || dataFim.HasValue)
            {
                ValidaDatas(dataInicio.HasValue ? dataInicio.Value : DataInicio, dataFim.HasValue ? dataFim.Value : DataFim);

                DataInicio = dataInicio.HasValue ? dataInicio.Value : DataInicio;
                DataFim = dataFim.HasValue ? dataFim.Value : DataFim;
            }

            if(DataOcorrencia.HasValue)
                DataOcorrencia = dataOcorrencia!.Value;
        }

        public void AlteraMovimentacaoFixaDiaria(ContaUsuario? contaUsuario, StatusMovimentacaoFixa? status, DateOnly? dataInicio, DateOnly? dataFim, int[]? ocorrenciaDiaria)
        {
            AlteraMovimentacaoFixa(contaUsuario,null,status,dataInicio,dataFim,null);
            if(ocorrenciaDiaria is not null)
            {
                DiasFixosDiarios!.Clear();
                ocorrenciaDiaria.ToList().ForEach(ms => DiasFixosDiarios.Add(new MovimentacaoFixaDiaria(this, ms)));
            }
        }
        private void ValidaDatas(DateOnly dataInicio, DateOnly dataFim)
        {
            MovimentacaoFixaValidacao.Verifica(dataInicio >= dataFim, MensagemMovimentacaoFixa.DATAS_INVALIDAS);
        }
        private void ValidaTipoFixo(TipoMovimentacaoFixa tipo)
        {
            MovimentacaoFixaValidacao.Verifica(!Enum.IsDefined(typeof(TipoMovimentacaoFixa), tipo), MensagemMovimentacaoFixa.TIPO_INVALIDO);
        }
        private void ValidaStatusMovimentacaoBase(StatusMovimentacao status)
        {
            MovimentacaoFixaValidacao.Verifica(status != StatusMovimentacao.Oculta, MensagemMovimentacaoFixa.MOVIMENTACAO_NAO_ESTA_OCULTA);
        }
        private void ValidaStatus(StatusMovimentacaoFixa status)
        {
            MovimentacaoFixaValidacao.Verifica(!Enum.IsDefined(typeof(StatusMovimentacaoFixa), status), "Status de movimentaçao fixa inválida.");
        }
    }
}
