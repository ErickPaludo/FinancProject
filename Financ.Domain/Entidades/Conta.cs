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
        public TiposStatusContas Status { get; private set; }
        public TiposContas TipoConta { get; private set; }
        private readonly List<ContasUsuarios> _contasUsuarios = new();
        public IReadOnlyCollection<ContasUsuarios> ContaUsuarios => _contasUsuarios;
        private readonly List<Convites> _convites = new();
        public IReadOnlyCollection<Convites> Convites => _convites;
        private Conta() { }

        public void AddUsuario(ContasUsuarios usuario) => _contasUsuarios.Add(usuario);
        public void AddConvite(Convites convite) => _convites.Add(convite);
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

        public void AtualizaConta(ContasUsuarios usuario, string? titulo, TiposStatusContas? status)
        {
            ContasValidacao.Verifica(usuario is null || usuario.Conta != this, MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
            ContasValidacao.Verifica(!usuario!.Status.Equals(TipoStatusContasUsuario.Ativo), MensagensBase.USUARIO_INATIVO_NAO_PODE_SER_ATUALIZADO);
            ContasValidacao.Verifica((usuario!.Acesso != TiposAcessos.Mestre), MensagensContas.ATUALIZA_CONTA_USUARIO_SEM_PERMISSAO);

            if (titulo != null)
                ValidaTitulo(titulo);

            if (status.HasValue)
                ValidaStatusConta(status.Value);
        }

        public void SairDaConta(ContasUsuarios? contaUsuario)
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
