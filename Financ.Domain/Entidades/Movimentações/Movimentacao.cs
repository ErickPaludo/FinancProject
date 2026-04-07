using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.Movimentações
{
    public class Movimentacao
    {
        public int Id { get; private set; }
        public TipoMovimentacao Tipo { get; private set; }
        public int IdConta { get; private set; }
        public int IdContaUsuario { get; private set; }
        public int IdCategoria { get; private set; }
        public int IdFixo { get; private set; }
        public decimal Valor { get; private set; }
        public TipoStatusMovimentacao Status { get; private set; } = TipoStatusMovimentacao.Pendente;
        public string Titulo { get; private set; }
        public string Observacao { get; private set; }
        public DateTime? DthrReg { get; private set; }
        public DateTime? DthrPagamento { get; private set; }
        public Conta Conta { get; private set; }
        public ContaUsuario ContaUsuario { get; private set; }
        public Categoria Categoria { get; private set; }
        public Movimentacao() { }
        public Movimentacao(int id, TipoMovimentacao tipo, ContaUsuario contaUsuario, Categoria? categoria, decimal valor, string titulo, string observacao, DateTime? dthrReg)
        {
            ValidaTipoMovimentacao(tipo);
            ValidaTitulo(titulo);
            ValidaObservacao(observacao);
            ValidaContaUsuario(contaUsuario);
            ValidaConta(contaUsuario.Conta);
            ValidaCategoria(categoria, contaUsuario.Conta);
            ValidaValor(valor);
            ValidaDatas(dthrReg);
          //  MovimentacaoValidacao.Verifica(tipo.Equals(TipoMovimentacao.Saida) && status.Equals(TipoStatusMovimentacao.Pago) && valor > contaUsuario.Conta!.Saldo, MensagemMovimentacao.SALDO_INSUFICIENTE);//nao valida aqui
        }
        private void ValidaStatusMovimentacao(TipoStatusMovimentacao status)
        {
            MovimentacaoValidacao.Verifica(!Enum.IsDefined(typeof(TipoStatusMovimentacao), status), MensagemMovimentacao.STATUS_INVALIDO);
            Status = status;
        }
        private void ValidaTipoMovimentacao(TipoMovimentacao tipo)
        {
            MovimentacaoValidacao.Verifica(!Enum.IsDefined(typeof(TipoMovimentacao), tipo), MensagemMovimentacao.TIPO_MOV_INVALIDO);
            Tipo = tipo;
        }
        private void ValidaTitulo(string titulo)
        {
            MovimentacaoValidacao.Verifica(string.IsNullOrWhiteSpace(titulo), MensagemMovimentacao.TITULO_OBRIGATORIO);
            MovimentacaoValidacao.Verifica(titulo.Length < 3 || titulo.Length > 80, MensagemMovimentacao.TITULO_LIMITE_CARACTERES);
            Titulo = titulo;
        }
        private void ValidaObservacao(string observacao)
        {
            MovimentacaoValidacao.Verifica(string.IsNullOrWhiteSpace(observacao) && observacao.Length < 255, MensagemMovimentacao.OBSERVACAO_LIMITE_CARACTERES);
            Observacao = observacao;
        }
        private void ValidaContaUsuario(ContaUsuario? contaUsuario)
        {
            MovimentacaoValidacao.Verifica(contaUsuario is null, MensagemMovimentacao.USUARIO_NAO_PERTENCE_A_CONTA);
            MovimentacaoValidacao.Verifica(!contaUsuario!.Status.Equals(TipoStatusContasUsuario.Ativo), MensagemMovimentacao.USUARIO_INATIVO);
            MovimentacaoValidacao.Verifica(contaUsuario.Expiracao > DateTime.UtcNow, MensagemMovimentacao.USUARIO_EXPIRADO);
            MovimentacaoValidacao.Verifica(!contaUsuario!.Acesso.Equals(TiposAcessos.Visualizador), MensagemMovimentacao.USUARIO_SEM_PERMISSAO);
            IdContaUsuario = contaUsuario!.Id;
        }
        private void ValidaConta(Conta? conta)
        {
            MovimentacaoValidacao.Verifica(conta is null, MensagemMovimentacao.CONTA_NAO_ENCONTRADA);
            IdConta = conta!.Id;
        }
        private void ValidaCategoria(Categoria? categoria, Conta conta)
        {
            MovimentacaoValidacao.Verifica(categoria != null && categoria!.Conta == conta, MensagemMovimentacao.CATEGORIA_NAO_PERTENCA_A_CONTA);
            IdCategoria = categoria!.Id;
        }
        private void ValidaValor(decimal valor)
        {
            MovimentacaoValidacao.Verifica(valor <= 0, MensagemMovimentacao.VALOR_DEVE_SER_MAIOR_QUE_ZERO);
            Valor = valor;
        }
        private void ValidaDatas(DateTime? dthrReg)
        {
            DthrReg = dthrReg is null ? DateTime.UtcNow : dthrReg;
        }
    }
}
