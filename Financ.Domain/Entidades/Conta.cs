using Financ.Domain.Enums;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades
{
    public sealed class Conta : BaseConta
    {
        public string Titulo { get; private set; }
        public TiposContas TipoConta { get; private set; }

        private readonly List<ContasUsuarios> _contasUsuarios = new();
        public IReadOnlyCollection<ContasUsuarios> ContaUsuarios => _contasUsuarios;

        private Conta() { }
      //  public ICollection<ContasUsuarios>? ContasUsuariosVinculados { get; set; }

        public void AddUsuario(ContasUsuarios usuario) => _contasUsuarios.Add(usuario);

        public Conta(string titulo)
        {
            ValidaTitulo(titulo);
            ContaPadrao();
        }
        public Conta(int id, string titulo)
        {
            ValidaTitulo(titulo);
            ContaPadrao();
            
            ContasValidacao.Verifica(id <= 0, MensagensBase.ID_IGUAL_MENOR_ZERO);
            Id = id;
        }
        private void ContaPadrao()
        {
            Status = TiposStatusContas.Ativo;
            TipoConta = TiposContas.Corrente;
            DthrReg = DateTime.Now;
          //  ContasUsuariosVinculados = new List<ContasUsuarios>();
        }
      
        private void ValidaTitulo(string titulo)
        {
            ContasValidacao.Verifica(string.IsNullOrWhiteSpace(titulo), MensagensContas.TITULO_OBRIGATORIO);
            ContasValidacao.Verifica(titulo.Length < 3 || titulo.Length > 100, MensagensContas.TITULO_TAMANHO_INVALIDO);
            Titulo = titulo;
        }
        private void ValidaStatus(TiposStatusContas status)
        {
            ContasValidacao.Verifica(!Enum.IsDefined(typeof(TiposStatusContas), status), MensagensBase.STATUS_INVALIDO);
            Status = status;
        }
        public void AtualizaConta(ContasUsuarios usuarios, string? titulo, TiposStatusContas? status)
        {
            ContasValidacao.Verifica(usuarios == null, MensagensBase.USUARIO_NAO_INFORMADO);
            ContasValidacao.Verifica((usuarios!.Acesso == TiposAcessos.Visualizador), MensagensContas.ATUALIZA_CONTA_USUARIO_SEM_PERMISSAO);

            if (titulo != null)
                ValidaTitulo(titulo);

            if (status is not null)
                ValidaStatus(status.Value);
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
