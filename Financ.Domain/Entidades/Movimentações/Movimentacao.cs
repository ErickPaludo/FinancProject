using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace Financ.Domain.Entidades.Movimentações
{
    public class Movimentacao
    {
        public int Id { get; private set; }
        public TipoMovimentacao Tipo { get; private set; }
        public int IdConta { get; private set; }
        public int IdUsuarioCriador { get; private set; }
        public int? IdUsuarioExecutor { get; private set; }
        public int? IdCategoria { get; private set; }
        public int IdFixo { get; private set; }
        public decimal Valor { get; private set; }
        public TipoStatusMovimentacao Status { get; private set; } = TipoStatusMovimentacao.Pendente;
        public string Titulo { get; private set; }
        public string? Observacao { get; private set; }
        public DateTime? DthrReg { get; private set; }
        public DateTime? DthrMovimentacao { get; private set; }
        public DateTime? DthrConclusao { get; private set; }
        [NotMapped]
        public bool Extorno { get; private set; }
        public Conta Conta { get; private set; }
        public ContaUsuario ContaUsuarioCriador{ get; private set; }
        public ContaUsuario? ContaUsuarioExecutor { get; private set; }
        public Categoria Categoria { get; private set; }
        private Movimentacao() { }

        public Movimentacao(TipoMovimentacao tipo, ContaUsuario? contaUsuario, Categoria? categoria, decimal valor, string titulo, string? observacao, DateTime? dthrMovimentacao, DateTime? dthrConclusao, bool concluido)
        {
            ValidaTipoMovimentacao(tipo);
            Tipo = tipo;

            Status = concluido ? TipoStatusMovimentacao.Concluido : TipoStatusMovimentacao.Pendente;

            ValidaTitulo(titulo);
            Titulo = titulo;

            ValidaObservacao(observacao);
            Observacao = observacao;

            ValidaContaUsuario(contaUsuario);
            IdUsuarioCriador = contaUsuario!.Id;
            ContaUsuarioCriador = contaUsuario;

            ValidaConta(contaUsuario!.Conta);
            IdConta = contaUsuario!.Conta!.Id;
            Conta = contaUsuario!.Conta;

            ValidaCategoria(categoria, contaUsuario.Conta);
            IdCategoria = categoria is null ? null : categoria!.Id;
            Categoria = categoria!;

            ValidaValor(valor);
            Valor = valor;

            DthrReg = DateTime.UtcNow; //data do sistema
            DthrMovimentacao = dthrMovimentacao is null ? DthrReg : dthrMovimentacao; //data em que a movimentacao deve/foi feita

            ValidaDataConclusao(dthrConclusao);
            DthrConclusao = dthrConclusao ?? dthrConclusao;
        }
        public void ExecutarMovimentacao(ContaUsuario? contaUsuario,DateTime? dthrConclusao = null)
        {
            ValidaContaUsuario(contaUsuario);
            ValidaConta(contaUsuario!.Conta);
            ValidaDataConclusao(dthrConclusao);

            MovimentacaoValidacao.Verifica(Status is TipoStatusMovimentacao.Concluido, MensagemMovimentacao.MOVIMENTACAO_COM_STATUS_IGUAL_NA_EXECUCAO);
            Status = TipoStatusMovimentacao.Concluido;
            ContaUsuarioExecutor = contaUsuario;
            IdUsuarioExecutor = contaUsuario.Id;
            ContaUsuarioExecutor = contaUsuario;
        }
        public void ExtornaMovimentacao(ContaUsuario? contaUsuario)
        {
            ValidaContaUsuario(contaUsuario);
            ValidaConta(contaUsuario!.Conta);

            MovimentacaoValidacao.Verifica(Status is TipoStatusMovimentacao.Pendente, MensagemMovimentacao.MOVIMENTACAO_COM_STATUS_IGUAL_NO_EXTORNO);

            DthrConclusao = null;
            Status = TipoStatusMovimentacao.Pendente;
            ContaUsuarioExecutor = contaUsuario;
            IdUsuarioExecutor = contaUsuario.Id;
            Extorno = true;
        }
        private void ValidaStatusMovimentacao(TipoStatusMovimentacao? status)
        {
            MovimentacaoValidacao.Verifica(status is not null && !Enum.IsDefined(typeof(TipoStatusMovimentacao), status), MensagemMovimentacao.STATUS_INVALIDO);
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
            MovimentacaoValidacao.Verifica(!contaUsuario!.Status.Equals(TipoStatusContasUsuario.Ativo), MensagemMovimentacao.USUARIO_INATIVO);
            MovimentacaoValidacao.Verifica(contaUsuario.Expiracao is not null && contaUsuario.Expiracao < DateTime.UtcNow, MensagemMovimentacao.USUARIO_EXPIRADO);
            MovimentacaoValidacao.Verifica(contaUsuario!.Acesso.Equals(TiposAcessos.Visualizador), MensagemMovimentacao.USUARIO_SEM_PERMISSAO);
        }
        private void ValidaConta(Conta? conta)
        {
            MovimentacaoValidacao.Verifica(conta is null, MensagemMovimentacao.CONTA_NAO_ENCONTRADA);
        }
        private void ValidaCategoria(Categoria? categoria, Conta conta)
        {
            MovimentacaoValidacao.Verifica(categoria is not null && categoria!.Conta != conta, MensagemMovimentacao.CATEGORIA_NAO_PERTENCA_A_CONTA);
        }
        private void ValidaValor(decimal valor)
        {
            MovimentacaoValidacao.Verifica(valor <= 0, MensagemMovimentacao.VALOR_DEVE_SER_MAIOR_QUE_ZERO);
        }

        private void ValidaDataConclusao(DateTime? dthrConclusao)
        {
            ContasValidacao.Verifica(dthrConclusao is not null && Status is not TipoStatusMovimentacao.Concluido, MensagemMovimentacao.MOVIMENTACAO_NAO_ESTA_CONCLUIDA);
            ContasValidacao.Verifica(dthrConclusao is not null && dthrConclusao < DthrMovimentacao, MensagemMovimentacao.DATAS_MOV_INVALIDAS);
        }
    }
}
