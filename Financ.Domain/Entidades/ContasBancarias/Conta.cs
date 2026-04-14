using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.ContasBancarias
{
    public sealed class Conta : BaseConta
    {
        public string Titulo { get; private set; }
        public TiposStatusContas Status { get; private set; }
        public TipoConta TipoConta { get; private set; }
        public decimal Saldo { get; set; }
        public Cor Cor { get; private set; }
        private readonly List<ContaUsuario> _contasUsuarios = new();
        public IReadOnlyCollection<ContaUsuario> ContaUsuarios => _contasUsuarios;
        private readonly List<Convite> _convites = new();
        public IReadOnlyCollection<Convite> Convites => _convites;
        private Conta() { }

        public void AddUsuario(ContaUsuario usuario) => _contasUsuarios.Add(usuario);
        public void AddConvite(Convite convite) => _convites.Add(convite);
        public Conta(int id, string titulo, string cor)
        {
            ValidaTitulo(titulo);
            ContaPadrao();
            Cor = new Cor(cor);
            ContasValidacao.Verifica(id <= 0, MensagensBase.ID_IGUAL_MENOR_ZERO);
            Id = id;
        }
        public Conta(string titulo, string? cor)
        {
            ValidaTitulo(titulo);
            ContaPadrao();
            Cor = new Cor(cor);
        }
        public void AtualizaConta(ContaUsuario? usuario, string? titulo, TiposStatusContas? status, string? cor = null)
        {
            ContasValidacao.Verifica(usuario is null || usuario.Conta != this, MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
            ContasValidacao.Verifica(!usuario!.Status.Equals(TipoStatusContasUsuario.Ativo), MensagensBase.USUARIO_INATIVO_NAO_PODE_SER_ATUALIZADO);
            ContasValidacao.Verifica((usuario!.Acesso != TiposAcessos.Mestre), MensagensContas.ATUALIZA_CONTA_USUARIO_SEM_PERMISSAO);

            if (cor != null)
                Cor = new Cor(cor);

            if (titulo != null)
                ValidaTitulo(titulo);

            if (status.HasValue)
                ValidaStatusConta(status.Value);
        }
        public void SairDaConta(ContaUsuario? contaUsuario)
        {
            ContasValidacao.Verifica(contaUsuario is null || contaUsuario.Conta != this, MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);

            contaUsuario!.SairDaConta();
            _contasUsuarios.Remove(contaUsuario);

            if (ContaUsuarios.Count() == 0)
                Status = TiposStatusContas.Inativo;
        }
        public bool ConviteEmAndamento(string idUsuario)
        {
            return Convites.Any(x => x.IdUsuarioDestinatario == idUsuario
            && DateTime.UtcNow <= x.Expiracao
            && x.Aceito == null);
        }
        public bool UsuarioPertenceConta(string idUsuario)
        {
            return ContaUsuarios.Any(x => x.IdUsuario == idUsuario);
        }
        public void ProcessaMovimentacao(Movimentacao movimentacao)
        {
            if (movimentacao.Status is TipoStatusMovimentacao.Concluido)
            {
                ContasValidacao.Verifica(movimentacao.Extorno, MensagensContas.NAO_PODE_PROCESSAR_MOVIMENTACAO_COM_EXTORNO);
                ContasValidacao.Verifica(movimentacao.Tipo.Equals(TipoMovimentacao.Saida) && movimentacao.Valor > Saldo, MensagensContas.SALDO_INSUFICIENTE);
                Saldo = movimentacao.Tipo.Equals(TipoMovimentacao.Entrada) ? Saldo + movimentacao.Valor : Saldo - movimentacao.Valor;
            }
        }
        public void ProcessaExtornoMovimentacao(Movimentacao movimentacao)
        {
            ContasValidacao.Verifica(!movimentacao.Extorno, MensagensContas.NAO_PODE_PROCESSAR_MOVIMENTACAO_SEM_EXTORNO);
            ContasValidacao.Verifica(movimentacao.Status is not TipoStatusMovimentacao.Pendente, MensagensContas.EXTORNO_DE_MOVIMENTACAO_COM_DATA_DE_CONCLUSAO);
            ContasValidacao.Verifica(movimentacao.DthrConclusao is not null, MensagensContas.EXTORNO_DE_MOVIMENTACAO_COM_DATA_DE_CONCLUSAO);
            ContasValidacao.Verifica(movimentacao.Tipo.Equals(TipoMovimentacao.Entrada) && movimentacao.Valor > Saldo, MensagensContas.SALDO_INSUFICIENTE);
            Saldo = movimentacao.Tipo.Equals(TipoMovimentacao.Entrada) ? Saldo - movimentacao.Valor : Saldo + movimentacao.Valor;
        }


        private void ContaPadrao()
        {
            Status = TiposStatusContas.Ativo;
            TipoConta = TipoConta.Corrente;
            DthrReg = DateTime.UtcNow;
        }
        private void ValidaTitulo(string titulo)
        {
            ContasValidacao.Verifica(string.IsNullOrWhiteSpace(titulo), MensagensContas.TITULO_OBRIGATORIO);
            ContasValidacao.Verifica(titulo.Length < 3 || titulo.Length > 100, MensagensContas.TITULO_TAMANHO_INVALIDO);
            Titulo = titulo;
        }
        private void ValidaStatusConta(TiposStatusContas status)
        {
            ContasValidacao.Verifica(!Enum.IsDefined(typeof(TiposStatusContas), status), MensagensBase.STATUS_INVALIDO);
            Status = status;
        }
        #region Linhas de credito Fase 3
        //private void ValidaFechamentoVencimento(int diaFechamento, int diaVencimento)
        //{
        //    ContasValidacao.Verifica(diaFechamento < 1 || diaFechamento > 16, MensagensContas.FECHAMENTO_INVALIDO);

        //    int diferencaDiasFechamento = diaVencimento - diaFechamento; //diferença entre o dia de fechamento e o dia de vencimento

        //    ContasValidacao.Verifica(diaVencimento <= diaFechamento, MensagensContas.VENCIMENTO_MENOR_FECHAMENTO);

        //    ContasValidacao.Verifica(diferencaDiasFechamento < 7, MensagensContas.VENCIMENTO_MINIMO_7_DIAS);

        //    ContasValidacao.Verifica(diferencaDiasFechamento >= 12, MensagensContas.VENCIMENTO_MAXIMO_12_DIAS);
        //    DiaFechamento = diaFechamento;
        //    DiaVencimento = diaVencimento;
        //}
        //private void ValidaCreditoLimite(double? creditoMaximo)
        //{
        //    ContasValidacao.Verifica(creditoMaximo == null, MensagensContas.ATUALIZA_CONTA_CREDITO_MAXIMO_NULO);
        //    ContasValidacao.Verifica(creditoMaximo <= 0, MensagensContas.CREDITO_MENOR_QUE_ZERO);
        //    CreditoMaximo = creditoMaximo;
        //}
        // Validacoes para atualizar conta
        //    if (diaFechamento is not null)
        //        ValidaFechamentoVencimento(diaFechamento.Value, DiaVencimento!.Value);

        //    if (diaVencimento is not null)
        //        ValidaFechamentoVencimento(DiaFechamento!.Value, diaVencimento.Value);

        //    if (!CreditoAtivo && creditoAtivo == true)
        //    {
        //        CreditoAtivo = creditoAtivo.Value;
        //        ContasValidacao.Verifica((DiaFechamento is null && !diaFechamento.HasValue) || (DiaVencimento is null && !diaVencimento.HasValue), MensagensContas.FECHAMENTO_INVALIDO);
        //        ValidaFechamentoVencimento(diaFechamento.Value, diaVencimento.Value);
        //    }

        //    if (CreditoAtivo || creditoAtivo == true)
        //    {
        //        if (diaFechamento is not null && diaVencimento is not null)
        //            ValidaFechamentoVencimento(diaFechamento.Value, diaVencimento.Value);


        //        if (creditoLimite is not null)
        //        {
        //            CreditoLimite = creditoLimite.Value;
        //            if (creditoLimite.Value)
        //            {
        //                if (CreditoMaximo is null)
        //                    ValidaCreditoLimite(creditoMaximo);
        //            }
        //            else
        //            {
        //                CreditoMaximo = null;
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
