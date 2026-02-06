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
        public DateTime Expiracao { get; private set; }
       // public Usuario Remetente { get; private set; }
       // public Usuario Destinatario { get; private set; }
        public Conta Contas { get; private set; }
        private Convites() { }
        public Convites(Conta conta, TiposAcessos acesso)
        {
            //ConvitesValidacao.Verifica(usuarioRemetente.Acesso != TiposAcessos.Mestre, MensagensConvite.USUARIO_SEM_PERMISSAO);
            ConvitesValidacao.Verifica(acesso == TiposAcessos.Mestre, MensagensConvite.CONTA_JA_POSSUI_UM_USUARIO_MASTER);
            ConvitesValidacao.Verifica(conta.Status != TiposStatus.Ativo, MensagensConvite.USUARIO_SEM_PERMISSAO);

            //ValidaUsuarios(usuarioRemetente.IdUsuario, usuarioDestinatario.IdUsuario);
            IdConta = conta.Id;
            Acesso = acesso;
            Expiracao = DateTime.Now.AddDays(7);
            Contas = conta;
           // Remetente = usuarioRemetente.Usuario;
          //  Destinatario = usuarioDestinatario;
        }
        private void ValidaUsuarios(string idUsuarioRemetente, string idUsuarioDestinatario)
        {
            ConvitesValidacao.Verifica(string.IsNullOrWhiteSpace(idUsuarioRemetente), MensagensConvite.USUARIO_REMETENTE_INVALIDO);
            ConvitesValidacao.Verifica(string.IsNullOrWhiteSpace(idUsuarioDestinatario), MensagensConvite.USUARIO_DESTINATARIO_INVALIDO);
            IdUsuarioRemetente = idUsuarioRemetente;
            IdUsuarioDestinatario = idUsuarioDestinatario;
        }
    }
}
