using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Financ.Domain.Entidades.ContasBancarias
{
    public sealed class ContaUsuario : BaseConta
    {
        public int IdConta { get; private set; }
        public string IdUsuario { get; private set; }
        public ETiposAcessos Acesso { get; private set; }
        public EStatusContasUsuario Status { get; private set; }
        public DateTime? Expiracao { get; private set; }
        public bool ContaFavorita { get; private set; } = false;
        public bool AutoSoma { get; private set; } = true;
        public Usuario? Usuario { get; private set; }

        public Conta Conta { get; private set; }

        #region Construtores
        public ContaUsuario() { }
        public ContaUsuario(Convite convite)
        {
            ArgumentNullException.ThrowIfNull(convite);

            ValidaContaBancaria(convite.Conta, convite.IdUsuarioDestinatario);
            Conta = convite.Conta;
            IdUsuario = convite.IdUsuarioDestinatario;
            DthrReg = DateTime.UtcNow;
            ValidaEnums(convite.Acesso, null);
            Acesso = convite.Acesso;
            Status = EStatusContasUsuario.Ativo;
            Usuario = convite.Destinatario;
            if (convite.ExpiracaoContaUsuario.HasValue)
            {
                Expiracao = DateTime.UtcNow.AddMinutes(convite.ExpiracaoContaUsuario.Value);
            }
        }
        public ContaUsuario(Conta conta, Usuario usuario)
        {
            Conta = conta;
            IdUsuario = usuario.Id;
            Usuario = usuario;
            DthrReg = DateTime.UtcNow;
            Status = EStatusContasUsuario.Ativo;
            Acesso = ETiposAcessos.Mestre;
        }
        #endregion

        #region Metodos Publicos
        public void AtualizaOutraContaUsuario(ContaUsuario? contasUsuarioRemetente, ETiposAcessos? acesso, EStatusContasUsuario? status, int? expiracao = null, bool? removerExpiracao = null)
        {

            #region Validação Remetente
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente == this, MensagensContasUsuarios.USUARIO_TENTA_SE_ATUALIZAR);
          
            #endregion

            ContasUsuariosValidacao.Verifica(ValidaUsuarioMestre(Acesso), MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO);

            ValidaEnums(acesso, status);


            if (acesso.HasValue)
            {
                ContasUsuariosValidacao.Verifica(
                   (Expiracao is not null ||
                    (expiracao.HasValue)) &&
                    ValidaUsuarioMestre(acesso.Value),
                    MensagensContasUsuarios.USUARIO_MESTRE_COM_TEMPO_LIMITE_JA_DEFINIDO);

                ContasUsuariosValidacao.Verifica(!ValidaPermissoeNaConta(acesso.Value), MensagensBase.LIMITE_USUARIOS_MESTRES);
                Acesso = acesso.Value;

            }

            if (status.HasValue)
            {
                ContasUsuariosValidacao.Verifica(!status.Value.Equals(EStatusContasUsuario.Ativo) && ValidaUsuarioMestre(Acesso), MensagensContasUsuarios.ATUALIZA_PARA_USUARIO_MESTRE_DIFERENTE_DE_ATIVO);
                Status = status.Value;
            }

            if (expiracao.HasValue)
            {
                ContasUsuariosValidacao.Verifica(ValidaExpiracao(expiracao.Value), MensagensContasUsuarios.TEMPO_MIN_EXPIRACAO);
                Expiracao = DateTime.UtcNow.AddMinutes(expiracao.Value);
            }

            if (removerExpiracao.HasValue)
            {
                ContasUsuariosValidacao.Verifica(expiracao.HasValue && (removerExpiracao.Value), MensagensContasUsuarios.CONFLITO_AO_EXPIRAR);
                Expiracao = removerExpiracao.Value ? null : Expiracao;
            }

        }
        public void SairDaConta()
        {
            ContasUsuariosValidacao.Verifica(
                Acesso.Equals(ETiposAcessos.Mestre)
                && Conta.ContaUsuarios.Any(x => !x.Acesso.Equals(ETiposAcessos.Mestre)
                && !x.IdUsuario.Equals(IdUsuario))
                && Conta.ContaUsuarios.Where(x => x.Acesso.Equals(ETiposAcessos.Mestre)).Take(2).Count() == 1,
                MensagensContasUsuarios.UNICO_USUARIO_MESTRE_NA_CONTA);

            ContasUsuariosValidacao.Verifica(Conta.Convites.Any(x => DateTime.UtcNow <= x.Expiracao && x.Aceito is null && x.IdUsuarioRemetente.Equals(IdUsuario)), MensagensContasUsuarios.USUARIO_POSSUI_CONVITES_EM_ANDAMENTO);

            Status = EStatusContasUsuario.Removido;


        }
        public void RemoverUsuarioDaConta(ContaUsuario? contasUsuarioRemetente)
        {
            ValidaUsuarioRemetenteMestreAtivoDaConta(contasUsuarioRemetente);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente == this, MensagensContasUsuarios.USUARIO_TENTA_SE_EXPULSAR);
            ContasUsuariosValidacao.Verifica(Acesso == ETiposAcessos.Mestre, MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_REMOVIDO);
            Status = EStatusContasUsuario.Removido;
        }
        public bool ValidaPermissoeNaConta(ETiposAcessos acessoDestinatario)
        {
            return !(acessoDestinatario.Equals(ETiposAcessos.Mestre) && Conta.ContaUsuarios.Where(x => x.Acesso.Equals(ETiposAcessos.Mestre) && x.Status.Equals(EStatusContasUsuario.Ativo)).Take(2).Count() == 2);
        }
        public bool ValidaUsuarioMestre(ETiposAcessos acesso)
        {
            return ETiposAcessos.Mestre.Equals(acesso);
        }
        public bool ExpiracaoPorAcesso(ETiposAcessos acesso)
        {
            return acesso.Equals(ETiposAcessos.Mestre);
        }
        public bool ValidaExpiracao(int minutos)
        {
            return minutos < 15;
        }
        public void ValidaSituacaoUsuarioParaConsulta()
        {
            ContasUsuariosValidacao.Verifica(Expiracao < DateTime.UtcNow, MensagensContasUsuarios.USUARIO_EXPIRADO);
        }
        public void RetornaParaConta(Convite convite)
        {
            ValidaContaBancaria(convite.Conta, convite.IdUsuarioDestinatario);
            ValidaEnums(convite.Acesso, null);
            Acesso = convite.Acesso;
            Status = EStatusContasUsuario.Ativo;
            if (convite.ExpiracaoContaUsuario.HasValue)
            {
                Expiracao = DateTime.UtcNow.AddMinutes(convite.ExpiracaoContaUsuario.Value);
            }
            else
            {
                Expiracao = null;
            }
        }
        public void FavoritarConta()
        {
            ContaFavorita = !ContaFavorita;
            DthrReg = DateTime.UtcNow;
        }
        public void AutoSomaConta()
        {
            AutoSoma = !AutoSoma;
            DthrReg = DateTime.UtcNow;
        }
        #endregion

        #region Metodos Privados
        private void ValidaEnums(ETiposAcessos? acesso, EStatusContasUsuario? status)
        {

            if (status.HasValue)
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(EStatusContasUsuario), status), MensagensBase.STATUS_INVALIDO);

            if (acesso.HasValue)
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(ETiposAcessos), acesso), MensagensContasUsuarios.ACESSO_INVALIDO);
        }
        private void ValidaContaBancaria(Conta conta, string idUsuario)
        {
            ContasUsuariosValidacao.Verifica(conta!.Status != EStatusContas.Ativo, MensagensContasUsuarios.CONTA_NAO_ESTA_ATIVA);
        }
        private void ValidaUsuarioRemetenteMestreAtivoDaConta(ContaUsuario? usuario)
        {
            ValidaUsuarioPertenceConta(usuario);
        }
        private void ValidaUsuarioPertenceConta(ContaUsuario? usuario)
        {
            ContasUsuariosValidacao.Verifica(usuario is null || usuario.Conta != Conta, MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        }
       
        #endregion



    }
}
