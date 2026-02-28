using Financ.Domain.Enums;
using Financ.Domain.Validacoes.Mensagens;
using Financ.Domain.Validacoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financ.Domain.Interfaces.InterfaceEntidades;
using System.Globalization;

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
        public Convites(TiposAcessos acesso, ContasUsuarios usuarioRemetente, string usuairoDestinatario)
        {
            ConvitesValidacao.Verifica(usuarioRemetente.Acesso != TiposAcessos.Mestre, MensagensConvite.USUARIO_SEM_PERMISSAO);
            ConvitesValidacao.Verifica(usuarioRemetente.Conta.Status != TiposStatusContas.Ativo, MensagensConvite.USUARIO_SEM_PERMISSAO);
            ConvitesValidacao.Verifica(!usuarioRemetente.ValidaPermissoeNaConta(acesso), MensagensConvite.LIMITE_USUARIOS_MASTER);
            
            IdUsuarioRemetente = usuarioRemetente.IdUsuario;
            IdUsuarioDestinatario = usuairoDestinatario;
            DataEnvio = DateTime.Now;
            IdConta = usuarioRemetente.Conta.Id;
            Acesso = acesso;
            Expiracao = DateTime.Now.AddDays(7);
            Contas = usuarioRemetente.Conta;
        }
   
        private void ValidaConviteAtivo(bool? aceito)
        {
            if (Aceito.HasValue)
            {
                string msg = Aceito.Value ? "aceito" : "rejeitado";
                ConvitesValidacao.Verifica(true, MensagensConvite.CONVITE_JA_VIZUALIZADO + msg);
            }
            ConvitesValidacao.Verifica(DateTime.Now > Expiracao, MensagensConvite.CONVITE_EXPIRADO);
        }
        public void AceitaConvite(bool aceito)
        {
            ValidaConviteAtivo(aceito);
            Aceito = aceito;
        }
        public void RevogaConvite(string idUsuarioRemetente)
        {
            ConvitesValidacao.Verifica(IdUsuarioRemetente != idUsuarioRemetente, MensagensConvite.CONVITE_EXPIRADO);
            ValidaConviteAtivo(Aceito);
        }
    }
}
