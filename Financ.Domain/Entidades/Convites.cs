using Financ.Domain.Enums;
using Financ.Domain.Validacoes.Mensagens;
using Financ.Domain.Validacoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financ.Domain.Interfaces.InterfaceEntidades;

namespace Financ.Domain.Entidades
{
    public sealed class Convites
    {
        public int Id { get; private set; }
        public string IdUsuarioRemetente { get; private set; }
        public string IdUsuarioDestinatario { get; private set; }
        public int IdConta { get; private set; }
        public TiposAcessos Acesso { get; private set; }
        public bool? Aceito { get; private set; }
        public DateTime DataEnvio { get; private set; }
        public DateTime Expiracao { get; private set; }

        public Conta Contas { get; private set; }
        private Convites() { }
        public Convites(Conta conta, TiposAcessos acesso, ContasUsuarios usuarioRemetente, Usuario usuarioDestinatario)
        {
            ConvitesValidacao.Verifica(usuarioRemetente.Acesso != TiposAcessos.Mestre, MensagensConvite.USUARIO_SEM_PERMISSAO);
            ConvitesValidacao.Verifica(acesso == TiposAcessos.Mestre, MensagensConvite.CONTA_JA_POSSUI_UM_USUARIO_MASTER);
            ConvitesValidacao.Verifica(conta.Status != TiposStatus.Ativo, MensagensConvite.USUARIO_SEM_PERMISSAO);

            ValidaUsuarios(usuarioRemetente.IdUsuario, usuarioDestinatario.IdUsuario);
            DataEnvio = DateTime.Now;
            IdConta = conta.Id;
            Acesso = acesso;
            Expiracao = DateTime.Now.AddDays(7);
            Contas = conta;
        }
        private void ValidaUsuarios(string idUsuarioRemetente, string idUsuarioDestinatario)
        {
            ConvitesValidacao.Verifica(string.IsNullOrWhiteSpace(idUsuarioRemetente), MensagensConvite.USUARIO_REMETENTE_INVALIDO);
            ConvitesValidacao.Verifica(string.IsNullOrWhiteSpace(idUsuarioDestinatario), MensagensConvite.USUARIO_DESTINATARIO_INVALIDO);
            IdUsuarioRemetente = idUsuarioRemetente;
            IdUsuarioDestinatario = idUsuarioDestinatario;
        }
        public void AceitaConvite(bool aceito)
        {
            if (Aceito.HasValue)
            {
                string msg = Aceito.Value ? "aceito" : "rejeitado";
                ConvitesValidacao.Verifica( true, MensagensConvite.CONVITE_JA_VIZUALIZADO + msg);
            }
            ConvitesValidacao.Verifica(DateTime.Now > Expiracao, MensagensConvite.CONVITE_EXPIRADO);
            Aceito = aceito;
        }
    }
}
