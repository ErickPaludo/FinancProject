using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financ.Domain.Entidades.Movimentações
{
    public class Movimentacao
    {
        public int Id { get; private set; }
        public TipoMovimentacao Tipo { get; private set; }
        public int IdConta { get; private set; }
        public int IdUsuarioCriador { get; private set; }
        public int? IdUsuarioExecutor { get; private set; }
        public int? IdFixo { get; private set; }
        public decimal Valor { get; private set; }
        public StatusMovimentacao Status { get; private set; } = StatusMovimentacao.Pendente;
        public string Titulo { get; private set; }
        public string? Observacao { get; private set; }
        public DateTime DthrReg { get; private set; }
        public DateTime DthrMovimentacao { get; private set; }
        public DateTime? DthrConclusao { get; private set; }
        public bool Editado { get; set; } = false;
        public bool Extorno { get; private set; }
        public Conta Conta { get; private set; }
        public ContaUsuario ContaUsuarioCriador { get; private set; }
        public ContaUsuario? ContaUsuarioExecutor { get; private set; }

        private readonly List<MovimentacaoCategoria> _movCategorais = new();
        public IReadOnlyCollection<MovimentacaoCategoria> CategoriasMovimentacao => _movCategorais;
        public MovimentacaoFixa? Fixa { get; private set; }
        #region Contrutores
        private Movimentacao() { }
        public Movimentacao(TipoMovimentacao tipo, ContaUsuario? contaUsuario, decimal valor, string titulo, string? observacao, DateTime? dthrMovimentacao, DateTime? dthrConclusao, bool concluido, MovimentacaoFixa? movimentacaoFixa = null)
        {
            if (movimentacaoFixa is not null)
            {
                IdFixo = movimentacaoFixa.Id;
                Fixa = movimentacaoFixa;
            }

            ValidaTipoMovimentacao(tipo);
            Tipo = tipo;

            Status = concluido ? StatusMovimentacao.Concluido : StatusMovimentacao.Pendente;

            ValidaTitulo(titulo);
            Titulo = titulo;

            ValidaObservacao(observacao);
            Observacao = observacao;

            ValidaContaUsuario(contaUsuario);
            IdUsuarioCriador = contaUsuario!.Id;
            ContaUsuarioCriador = contaUsuario;


            IdUsuarioExecutor = concluido ? contaUsuario.Id : null;
            ContaUsuarioExecutor = concluido ? contaUsuario : null;

            ValidaConta(contaUsuario!.Conta);
            IdConta = contaUsuario!.Conta!.Id;
            Conta = contaUsuario!.Conta;

            ValidaValor(valor);
            Valor = valor;

            DthrReg = DateTime.UtcNow; //data do sistema
            DthrMovimentacao = dthrMovimentacao is null ? DthrReg : dthrMovimentacao.Value; //data em que a movimentacao deve/foi feita

            ValidaDataConclusao(dthrConclusao);
            DthrConclusao = concluido ?( dthrConclusao ?? dthrMovimentacao) : null;
        }
        #endregion

        #region Metodos Publicos

        public void AdicionarCategoria(MovimentacaoCategoria movCategoria)
        {
            _movCategorais.Add(movCategoria);
        }
        public void ExecutarMovimentacao(ContaUsuario? contaUsuario, DateTime? dthrConclusao = null)
        {
            ValidaContaUsuario(contaUsuario);
            ValidaConta(contaUsuario!.Conta);
            MovimentacaoValidacao.Verifica(Status is StatusMovimentacao.Concluido, MensagemMovimentacao.MOVIMENTACAO_COM_STATUS_IGUAL_NA_EXECUCAO);
            Status = StatusMovimentacao.Concluido;
            ValidaDataConclusao(dthrConclusao);

            ContaUsuarioExecutor = contaUsuario;
            IdUsuarioExecutor = contaUsuario.Id;
            ContaUsuarioExecutor = contaUsuario;

            DthrConclusao = dthrConclusao ?? DthrMovimentacao;

        }
        public void ExtornaMovimentacao(ContaUsuario? contaUsuario)
        {
            ValidaExtorno(contaUsuario);

            DthrConclusao = null;
            Status = StatusMovimentacao.Pendente;
            ContaUsuarioExecutor = contaUsuario;
            IdUsuarioExecutor = contaUsuario!.Id;
            Extorno = true;
        }
        public void ExcluiMovimentacao(ContaUsuario? contaUsuario)
        {
            ValidaContaUsuario(contaUsuario);
            ValidaConta(contaUsuario!.Conta);
            Extorno = Status is StatusMovimentacao.Concluido ? true : false;
            Status = StatusMovimentacao.Excluido;
        }
        public void AlterarMovimentacao(ContaUsuario? contaUsuario, decimal? valor, TipoMovimentacao? tipo, string? titulo, string? observacao, DateTime? dthrMovimentacao, DateTime? dthrConclusao)
        {
            ValidaContaUsuario(contaUsuario);
            ValidaConta(contaUsuario!.Conta);

            if (titulo is not null)
            {
                ValidaTitulo(titulo);
                Titulo = titulo;
            }
            if (observacao is not null)
            {
                ValidaObservacao(observacao);
                Observacao = observacao;
            }
            if (dthrMovimentacao is not null)
            {
                DthrMovimentacao = dthrMovimentacao.Value;
            }
            if (dthrConclusao is not null)
            {
                MovimentacaoValidacao.Verifica(Status is not StatusMovimentacao.Concluido, MensagemMovimentacao.MOVIMENTACAO_NAO_ESTA_CONCLUIDA);
                DthrConclusao = dthrConclusao;
            }
            if (dthrMovimentacao is not null && dthrConclusao is not null)
            {
                ValidaDataConclusao(DthrConclusao);
            }
            if (valor is not null)
            {
                ValidaValor(valor.Value);
                MovimentacaoValidacao.Verifica(Status is StatusMovimentacao.Concluido, MensagemMovimentacao.NAO_PODE_ALTERAR_VALOR_DE_MOVIMENTACAO_CONCLUIDA);
                Valor = valor.Value;
            }
            if (tipo is not null)
            {
                ValidaTipoMovimentacao(tipo.Value);
                MovimentacaoValidacao.Verifica(Status is StatusMovimentacao.Concluido, MensagemMovimentacao.NAO_PODE_ALTERAR_TIPO_DE_MOVIMENTACAO_CONCLUIDA);
                Tipo = tipo.Value;
            }
            Editado = true;
        }
        public void AlteraCategoriaMovimentacao(ContaUsuario? contaUsuario)
        {
            ValidaContaUsuario(contaUsuario);
        }
        public void CriaMovimentacaoFixa(MovimentacaoFixa fixa)
        {
            Status = StatusMovimentacao.Oculta;
        }
        #endregion
        #region Metodos Privados
        private void ValidaExtorno(ContaUsuario? contaUsuario)
        {
            ValidaContaUsuario(contaUsuario);
            ValidaConta(contaUsuario!.Conta);
            MovimentacaoValidacao.Verifica(Status is StatusMovimentacao.Pendente, MensagemMovimentacao.MOVIMENTACAO_COM_STATUS_IGUAL_NO_EXTORNO);
        }
        private void ValidaStatusMovimentacao(StatusMovimentacao? status)
        {
            MovimentacaoValidacao.Verifica(status is not null && !Enum.IsDefined(typeof(StatusMovimentacao), status), MensagemMovimentacao.STATUS_INVALIDO);
        }
        private void ValidaTipoMovimentacao(TipoMovimentacao tipo)
        {
            MovimentacaoValidacao.Verifica(!Enum.IsDefined(typeof(TipoMovimentacao), tipo), MensagemMovimentacao.TIPO_MOV_INVALIDO);
        }
        private void ValidaTitulo(string titulo)
        {
            MovimentacaoValidacao.Verifica(string.IsNullOrWhiteSpace(titulo), MensagemMovimentacao.TITULO_OBRIGATORIO);
            MovimentacaoValidacao.Verifica(titulo.Length < 3 || titulo.Length > 80, MensagemMovimentacao.TITULO_LIMITE_CARACTERES);
        }
        private void ValidaObservacao(string? observacao)
        {
            MovimentacaoValidacao.Verifica(observacao is not null && !string.IsNullOrWhiteSpace(observacao) && observacao.Length > 255, MensagemMovimentacao.OBSERVACAO_LIMITE_CARACTERES);
        }
        private void ValidaContaUsuario(ContaUsuario? contaUsuario)
        {
            MovimentacaoValidacao.Verifica(contaUsuario is null, MensagemMovimentacao.USUARIO_NAO_PERTENCE_A_CONTA);
            MovimentacaoValidacao.Verifica(!contaUsuario!.Status.Equals(StatusContasUsuario.Ativo), MensagemMovimentacao.USUARIO_INATIVO);
            MovimentacaoValidacao.Verifica(contaUsuario.Expiracao is not null && contaUsuario.Expiracao < DateTime.UtcNow, MensagemMovimentacao.USUARIO_EXPIRADO);
            MovimentacaoValidacao.Verifica(contaUsuario!.Acesso.Equals(TiposAcessos.Visualizador), MensagemMovimentacao.USUARIO_SEM_PERMISSAO);
        }
        private void ValidaConta(Conta? conta)
        {
            MovimentacaoValidacao.Verifica(conta is null, MensagemMovimentacao.CONTA_NAO_ENCONTRADA);
        }
        private void ValidaValor(decimal valor)
        {
            MovimentacaoValidacao.Verifica(valor <= 0, MensagemMovimentacao.VALOR_DEVE_SER_MAIOR_QUE_ZERO);
        }
        private void ValidaDataConclusao(DateTime? dthrConclusao)
        {
            MovimentacaoValidacao.Verifica(dthrConclusao is not null && Status is not StatusMovimentacao.Concluido, MensagemMovimentacao.MOVIMENTACAO_NAO_ESTA_CONCLUIDA);
            MovimentacaoValidacao.Verifica(dthrConclusao is not null && dthrConclusao < DthrMovimentacao, MensagemMovimentacao.DATAS_MOV_INVALIDAS);
        }
        #endregion
    }
}
