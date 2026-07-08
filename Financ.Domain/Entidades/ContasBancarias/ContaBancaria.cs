using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Objetos_de_Valor.Titulo;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;


namespace Financ.Domain.Entidades.ContasBancarias
{
    public sealed class ContaBancaria : EntidadeBase
    {
        public TituloConta Titulo { get; private set; }
        public EStatusContas Status { get; private set; }
        public ETipoConta TipoConta { get; private set; }
        public Saldo Saldo { get; private set; }
        public Cor Cor { get; private set; }

        #region Relacionamento com ContasUsuarios
        private readonly List<ContaUsuario> _contasUsuarios = new();
        public IReadOnlyCollection<ContaUsuario> ContaUsuarios => new List<ContaUsuario>();
        #endregion

        //Questionavel
        #region Relacionamento com Convites 
        private readonly List<Convite> _convites = new();
        public IReadOnlyCollection<Convite> Convites => _convites;
        #endregion
        private ContaBancaria() { }

        public ContaBancaria(TituloConta titulo, string? cor)
        {
            ValidaNulo.Verifica(titulo, MensagensBase.TITULO_NULO);
            Titulo = titulo;
            Status = EStatusContas.Ativo;
            TipoConta = ETipoConta.Corrente;
            Cor = new Cor(cor);
        }
        public void AtualizaConta(string? titulo, EStatusContas? status, string? cor = null)
        {

            if (cor != null)
                Cor = new Cor(cor);

            if (titulo is not null)
            {
                Titulo = TituloConta.Create(titulo);
            }

            if (status.HasValue)
                ValidaStatusConta(status.Value);
        }
        //public bool ConviteEmAndamento(string idUsuario)
        //{
        //    return Convites.Any(x => x.IdUsuarioDestinatario == idUsuario
        //    && DateTime.UtcNow <= x.Expiracao
        //    && x.Aceito == null);
        //}
        //public bool UsuarioPertenceConta(string idUsuario)
        //{
        //   return ContaUsuarios.Any(x => x.IdUsuario == idUsuario && (x.Expiracao is null || x.Expiracao >= DateTime.UtcNow));
        //} 
        public void ProcessaMovimentacao(Movimentacao movimentacao)
        {
            if (movimentacao.EhSaida())
                DebitaSaldo(movimentacao.Saldo);
            else
                AdicionaSaldo(movimentacao.Saldo);
                AdicionaSaldo(movimentacao.Saldo);
        }
        public void ProcessaExtorno(Movimentacao movimentacao)
        {
            if (movimentacao.EhSaida())
                AdicionaSaldo(movimentacao.Saldo);
            else
                DebitaSaldo(movimentacao.Saldo);
        }
        private void DebitaSaldo(Saldo debito)
        {
            ContasValidacao.Verifica(Saldo.Subtrai(debito).Valor < 0, MensagensConta.SALDO_INSUFICIENTE);
            Saldo = Saldo.Subtrai(debito);
        }
        private void AdicionaSaldo(Saldo saldo)
        {
            Saldo = Saldo.Soma(saldo);
        }
        //public void ProcessaFatura(Credito credito)
        //{
        //    ProcessaMovimentacao(credito.movimentacao);
        //    credito.ProecessaMovimentacao(credito.movimentacao);
        //}
        //public void ProcessaExtornoMovimentacao(Movimentacao movimentacao)
        //{
        //    ContasValidacao.Verifica(!movimentacao.Extorno, MensagensContas.NAO_PODE_PROCESSAR_MOVIMENTACAO_SEM_EXTORNO);
        //    ContasValidacao.Verifica(movimentacao.Status is not EStatusMovimentacao.Pendente, MensagensContas.EXTORNO_DE_MOVIMENTACAO_COM_DATA_DE_CONCLUSAO);
        //    ContasValidacao.Verifica(movimentacao.DthrConclusao is not null, MensagensContas.EXTORNO_DE_MOVIMENTACAO_COM_DATA_DE_CONCLUSAO);
        //    ContasValidacao.Verifica(movimentacao.Tipo.Equals(ETipoMovimentacao.Entrada) && movimentacao.Valor > Saldo, MensagensContas.SALDO_INSUFICIENTE);
        //    Saldo = movimentacao.Tipo.Equals(ETipoMovimentacao.Entrada) ? Saldo - movimentacao.Valor : Saldo + movimentacao.Valor;
        //}

        //public void RemoverMovimentacao(Movimentacao movimentacao)
        //{
        //    if (movimentacao.Extorno)
        //    {
        //        if (movimentacao.Tipo.Equals(ETipoMovimentacao.Entrada))
        //        {
        //            ContasValidacao.Verifica(movimentacao.Valor > Saldo, MensagensContas.SALDO_INSUFICIENTE);
        //            Saldo -= movimentacao.Valor;
        //        }
        //        else
        //            Saldo += movimentacao.Valor;
        //    }
        //}

        private void ValidaStatusConta(EStatusContas status)
        {
            ContasValidacao.Verifica(!Enum.IsDefined(typeof(EStatusContas), status), MensagensBase.STATUS_INVALIDO);
            Status = status;
        }

    }
}
