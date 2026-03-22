using Financ.Domain.Enums;
using Financ.Domain.Interfaces.InterfaceEntidades;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        public int? ExpiracaoContaUsuario { get; private set; }

        public DateTime DataEnvio { get; private set; }
        public DateTime Expiracao { get; private set; }
        public string? Observacao { get; private set; }

        public Conta Conta { get; private set; }

        private Convites() { }
        public Convites(TiposAcessos acesso, ContasUsuarios? usuarioRemetente, Usuario? usuairoDestinatario, int? expiracaoContaUsuario = null)
        {
            ConvitesValidacao.Verifica(!Enum.IsDefined(typeof(TiposAcessos), acesso), MensagensContasUsuarios.ACESSO_INVALIDO);

            ConvitesValidacao.Verifica(usuarioRemetente is null, MensagensConvite.USUARIO_DESTINATARIO_NAO_ENCONTRADO);
            ConvitesValidacao.Verifica(usuairoDestinatario is null, MensagensConvite.USUARIO_DESTINATARIO_NAO_ENCONTRADO);

            ConvitesValidacao.Verifica(usuarioRemetente!.Acesso != TiposAcessos.Mestre, MensagensConvite.USUARIO_SEM_PERMISSAO);
            ConvitesValidacao.Verifica(usuarioRemetente.Status != TipoStatusContasUsuario.Ativo, MensagensConvite.USUARIO_CONTA_REMETENTE_INATIVO);
            ConvitesValidacao.Verifica(usuarioRemetente.Conta.Status != TiposStatusContas.Ativo, MensagensContas.CONTA_INATIVA);
            ConvitesValidacao.Verifica(!usuarioRemetente.ValidaPermissoeNaConta(acesso), MensagensBase.LIMITE_USUARIOS_MESTRES);

            ConvitesValidacao.Verifica(usuarioRemetente.Conta.UsuarioPertenceConta(usuairoDestinatario!.IdUsuario), MensagensConvite.USUARIO_JA_PERTENCE_A_CONTA);
            ConvitesValidacao.Verifica(usuarioRemetente.Conta.ConviteEmAndamento(usuairoDestinatario!.IdUsuario), MensagensConvite.CONVITE_EM_ANDAMENTO);

            if (expiracaoContaUsuario.HasValue)
            {
                ConvitesValidacao.Verifica(usuarioRemetente.ExpiracaoPorAcesso(acesso), MensagensContasUsuarios.MESTRE_NAO_POSSUI_TEMPO_LIMITE);
                ConvitesValidacao.Verifica(usuarioRemetente.ValidaExpiracao(expiracaoContaUsuario.Value), MensagensContasUsuarios.TEMPO_MIN_EXPIRACAO);
            }

            IdUsuarioRemetente = usuarioRemetente.IdUsuario;
            IdUsuarioDestinatario = usuairoDestinatario!.IdUsuario;
            DataEnvio = DateTime.Now;
            IdConta = usuarioRemetente.Conta.Id;
            Acesso = acesso;
            Expiracao = DateTime.Now.AddDays(7);
            Conta = usuarioRemetente.Conta;
            ExpiracaoContaUsuario = expiracaoContaUsuario;
        }

        private void ValidaConviteAtivo(bool? aceito)
        {
            if (Aceito.HasValue)
            {
                string msg = Aceito.Value ? "aceito" : "rejeitado";
                ConvitesValidacao.Verifica(true, MensagensConvite.CONVITE_JA_VISUALIZADO + msg);
            }
            ConvitesValidacao.Verifica(DateTime.Now > Expiracao, MensagensConvite.CONVITE_EXPIRADO);
        }
        public void AceitaConvite(bool aceito)
        {
            ValidaConviteAtivo(aceito);
            Aceito = aceito;
        }
        public void InsereObservacao(string observacao)
        {
            Observacao = observacao;
        }
        public void RevogaConvite(string idUsuarioRemetente)
        {
            ConvitesValidacao.Verifica(IdUsuarioRemetente != idUsuarioRemetente, MensagensConvite.CONVITE_EXPIRADO);
            ValidaConviteAtivo(Aceito);
        }

    }
}
