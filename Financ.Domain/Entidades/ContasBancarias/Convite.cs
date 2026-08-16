using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Convites;
using Financ.Domain.Objetos_de_Valor.ContaUsuario;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;

namespace Financ.Domain.Entidades.ContasBancarias
{
    public sealed class Convite : EntidadeBase
    {
        public ContaBancaria ContaBancaria {  get; }
        public ContaUsuario Remetente { get; }
        public Usuario Destinatario { get; }


        public ETiposAcessos Acesso { get; private set; }
        public EStatusConvite Status { get; private set; } = EStatusConvite.Pendente;
        public ExpiracaoConvite ExpiracaoConvite { get; } = ExpiracaoConvite.Create();
        public ExpiracaoContaUsuario? ExpiracaoContaUsuario { get; private set; }

        private Convite(ETiposAcessos acesso, ContaUsuario remetente, Usuario destinatario, ExpiracaoContaUsuario? expiracaoContaUsuario)
        {
            ContaBancaria contaBancaria = remetente.ContaBancaria;
            ValidaNulo.Verifica(remetente, MensagensBase.REMETENTE_NULO);
            ValidaNulo.Verifica(destinatario, MensagensBase.DESTINATARIO_NULO);
            ConvitesValidacao.Verifica(!Enum.IsDefined(typeof(ETiposAcessos), acesso), MensagensContaUsuario.ACESSO_INVALIDO);

            //Precisa verificar a quantidade de usuarios mestres na conta
            //Precisa verificar se o limite nao foi alcancado na conta

            ConvitesValidacao.Verifica(contaBancaria.Acessos.DisposicaoAcessos(contaBancaria.QuantidadeUsuarios), MensagensConvite.LIMITE_ACESSOS(contaBancaria.Acessos.MaxUsuario));

            ContaBancaria = remetente.ContaBancaria;
            Acesso = acesso;
            Remetente = remetente;
            Destinatario  = destinatario;
            ExpiracaoContaUsuario = expiracaoContaUsuario;
        }

        public static Convite Create(ETiposAcessos acesso, ContaUsuario usuarioRemetente, Usuario usuairoDestinatario, ExpiracaoContaUsuario? expiracaoContaUsuario)
        {
            return new Convite(acesso, usuarioRemetente, usuairoDestinatario, expiracaoContaUsuario);
        }

        public void AceitaConvite(bool aceito)
        {
            if (ConviteAtivo())
                Status = aceito ? EStatusConvite.Aceito : EStatusConvite.Recusado;
        }
        public void RevogaConvite()
        {
            if (ConviteAtivo())
                Status = EStatusConvite.Revogado;
        }
        private bool ConviteAtivo()
        {
            if (Status != EStatusConvite.Pendente)
                return false;

            if (ExpiracaoConvite is not null)
                return !ExpiracaoConvite.EstaExpirado();

            return true;
        }

    }
}
