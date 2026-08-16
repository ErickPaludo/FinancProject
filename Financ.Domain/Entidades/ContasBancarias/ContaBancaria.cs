using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Objetos_de_Valor.ContaBancaria;
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
        public LimiteAcessos Acessos { get; private set; }
        public EStatusContas Status { get; private set; }
        public ETipoConta TipoConta { get; private set; }
        public Saldo Saldo { get; private set; }
        public Cor Cor { get; private set; }

        #region Relacionamento com ContasUsuarios
        private readonly List<ContaUsuario> _contasUsuarios = new();
        public IReadOnlyCollection<ContaUsuario> ContaUsuarios => new List<ContaUsuario>();
        #endregion

        public int QuantidadeUsuarios => ContaUsuarios.Count(x => x.Expiracao is null || !x.Expiracao.EstaExpirado());

        //Questionavel
        #region Relacionamento com Convites 
        private readonly List<Convite> _convites = new();
        public IReadOnlyCollection<Convite> Convites => _convites;
        #endregion
        private ContaBancaria() { }

        private ContaBancaria(TituloConta titulo, Cor? cor)
        {
            ValidaNulo.Verifica(titulo, MensagensBase.TITULO_NULO);
            Titulo = titulo;
            Status = EStatusContas.Ativo;
            TipoConta = ETipoConta.Corrente;
            Acessos = LimiteAcessos.Create();
            Cor = cor ??= Cor.Create("#1d293db3");
        }

        public static ContaBancaria Create(TituloConta titulo, Cor? cor) => new ContaBancaria(titulo, cor);

        public void AtualizaConta(TituloConta? titulo,EStatusContas? status,LimiteAcessos? acessos,Cor? cor)
        {
            var houveAlteracao = false;

            if (status is EStatusContas novoStatus && Status != novoStatus)
            {
                ValidaStatusConta(novoStatus);
                houveAlteracao = true;
            }

            if (cor is not null && Cor != cor)
            {
                Cor = cor;
                houveAlteracao = true;
            }

            if (acessos is not null && Acessos != acessos)
            {
                Acessos = acessos;
                houveAlteracao = true;
            }

            if (titulo is not null && Titulo != titulo)
            {
                Titulo = titulo;
                houveAlteracao = true;
            }

            if (houveAlteracao)
                DataHoraAlteracao = DateTime.UtcNow;
        }
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
            ContasUsuariosValidacao.Verifica(movimentacao.Status != EStatusMovimentacao.Concluida, MensagensConta.IMPOSSIVEL_EXTORNAR);
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
        private void ValidaStatusConta(EStatusContas status)
        {
            ContasValidacao.Verifica(!Enum.IsDefined(typeof(EStatusContas), status), MensagensBase.STATUS_INVALIDO);
            Status = status;
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
        //public void ProcessaFatura(Credito credito)
        //{
        //    ProcessaMovimentacao(credito.movimentacao);
        //    credito.ProecessaMovimentacao(credito.movimentacao);
        //}
       

    }
}
