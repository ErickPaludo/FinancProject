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
        public TiposAcessos Acesso { get; private set; }
        public StatusContasUsuario Status { get; private set; }
        public DateTime? Expiracao { get; private set; }
        public bool ContaFavorita { get; private set; } = false;
        public Usuario Usuario { get; private set; }

        public Conta Conta { get; private set; }

        #region Construtores
        public ContaUsuario() { }
        public ContaUsuario(int id, Conta conta, string idUsuario, TiposAcessos acesso, StatusContasUsuario? status = null)
        {
            ContasUsuariosValidacao.Verifica(id <= 0, MensagensBase.ID_IGUAL_MENOR_ZERO);
            Id = id;
            // ContasUsuariosValidacao.Verifica(conta is null, MensagensContasUsuarios.CONTA_NAO_PODE_SER_NULA);
            // ContasUsuariosValidacao.Verifica(conta!.Status != TiposStatusContas.Ativo, MensagensContasUsuarios.CONTA_NAO_ESTA_ATIVA);
            ContasUsuariosValidacao.Verifica(string.IsNullOrEmpty(idUsuario), MensagensContasUsuarios.IDUSUARIO_INVALIDO);
            Conta = conta;
            //Usuario = usuario!;
            IdUsuario = idUsuario;
            DthrReg = DateTime.UtcNow; ValidaEnums(acesso, status);
            Status = status.HasValue ? status.Value : StatusContasUsuario.Ativo;
            Acesso = acesso;
        }
        public ContaUsuario(Convite convite)
        {
            ContasUsuariosValidacao.Verifica(convite is null, MensagensContasUsuarios.CONVITE_NAO_PODE_SER_NULO);
            ValidaContasUsuarios(convite!.Conta, convite.IdUsuarioDestinatario);
            Conta = convite!.Conta;
            IdUsuario = convite.IdUsuarioDestinatario;
            DthrReg = DateTime.UtcNow;
            ValidaEnums(convite.Acesso, null);

            Acesso = convite.Acesso;


            Status = StatusContasUsuario.Ativo;

            if (convite.ExpiracaoContaUsuario.HasValue)
            {
                Expiracao = DateTime.UtcNow.AddMinutes(convite.ExpiracaoContaUsuario.Value);
            }
        }
        public ContaUsuario(Conta conta, string idUasuario)
        {
            ValidaContasUsuarios(conta, idUasuario);
            Conta = conta;
            IdUsuario = idUasuario;
            DthrReg = DateTime.UtcNow;
            Status = StatusContasUsuario.Ativo;
            Acesso = TiposAcessos.Mestre;
        }
        #endregion

        #region Metodos Publicos
        public void AtualizaOutraContaUsuario(ContaUsuario? contasUsuarioRemetente, TiposAcessos? acesso, StatusContasUsuario? status, int? expiracao = null, bool? removerExpiracao = null)
        {

            #region Validação Remetente
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente is null || contasUsuarioRemetente.Conta != Conta, MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente == this, MensagensContasUsuarios.USUARIO_TENTA_SE_ATUALIZAR);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente!.Acesso != TiposAcessos.Mestre, MensagensContasUsuarios.ACESSO_NEGADO);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente.Status != StatusContasUsuario.Ativo, MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
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
                ContasUsuariosValidacao.Verifica(!status.Value.Equals(StatusContasUsuario.Ativo) && ValidaUsuarioMestre(Acesso), MensagensContasUsuarios.ATUALIZA_PARA_USUARIO_MESTRE_DIFERENTE_DE_ATIVO);
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
                Acesso.Equals(TiposAcessos.Mestre)
                && Conta.ContaUsuarios.Any(x => !x.Acesso.Equals(TiposAcessos.Mestre)
                && !x.IdUsuario.Equals(IdUsuario))
                && Conta.ContaUsuarios.Where(x => x.Acesso.Equals(TiposAcessos.Mestre)).Take(2).Count() == 1,
                MensagensContasUsuarios.UNICO_USUARIO_MESTRE_NA_CONTA);

            ContasUsuariosValidacao.Verifica(Conta.Convites.Any(x => DateTime.UtcNow <= x.Expiracao && x.Aceito is null && x.IdUsuarioRemetente.Equals(IdUsuario)), MensagensContasUsuarios.USUARIO_POSSUI_CONVITES_EM_ANDAMENTO);

            Status = StatusContasUsuario.Removido;


        }
        public void RemoverUsuarioDaConta(ContaUsuario? contasUsuarioRemetente)
        {
            ValidaUsuarioRemetenteMestreAtivoDaConta(contasUsuarioRemetente);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente == this, MensagensContasUsuarios.USUARIO_TENTA_SE_EXPULSAR);
            ContasUsuariosValidacao.Verifica(Acesso == TiposAcessos.Mestre, MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_REMOVIDO);
            Status = StatusContasUsuario.Removido;
        }
        public bool ValidaPermissoeNaConta(TiposAcessos acessoDestinatario)
        {
            return !(acessoDestinatario.Equals(TiposAcessos.Mestre) && Conta.ContaUsuarios.Where(x => x.Acesso.Equals(TiposAcessos.Mestre) && x.Status.Equals(StatusContasUsuario.Ativo)).Take(2).Count() == 2);
        }
        public bool ValidaUsuarioMestre(TiposAcessos acesso)
        {
            return TiposAcessos.Mestre.Equals(acesso);
        }
        public bool ExpiracaoPorAcesso(TiposAcessos acesso)
        {
            return acesso.Equals(TiposAcessos.Mestre);
        }
        public bool ValidaExpiracao(int minutos)
        {
            return minutos < 15;
        }
        public void ValidaSituacaoUsuarioParaConsulta()
        {
            ContasUsuariosValidacao.Verifica(Status is not StatusContasUsuario.Ativo, MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
            ContasUsuariosValidacao.Verifica(Expiracao < DateTime.UtcNow, MensagensContasUsuarios.USUARIO_EXPIRADO);
        }
        public void RetornaParaConta(Convite convite)
        {
            ValidaContasUsuarios(convite.Conta, convite.IdUsuarioDestinatario);
            ValidaEnums(convite.Acesso, null);
            Acesso = convite.Acesso;
            Status = StatusContasUsuario.Ativo;
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
        #endregion

        #region Metodos Privados
        private void ValidaEnums(TiposAcessos? acesso, StatusContasUsuario? status)
        {

            if (status.HasValue)
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(StatusContasUsuario), status), MensagensBase.STATUS_INVALIDO);

            if (acesso.HasValue)
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(TiposAcessos), acesso), MensagensContasUsuarios.ACESSO_INVALIDO);
        }
        private void ValidaContasUsuarios(Conta conta, string idUsuario)
        {
            ContasUsuariosValidacao.Verifica(conta is null, MensagensContasUsuarios.CONTA_NAO_PODE_SER_NULA);
            ContasUsuariosValidacao.Verifica(conta!.Status != StatusContas.Ativo, MensagensContasUsuarios.CONTA_NAO_ESTA_ATIVA);
            ContasUsuariosValidacao.Verifica(string.IsNullOrEmpty(idUsuario), MensagensContasUsuarios.IDUSUARIO_INVALIDO);

        }
        private void ValidaUsuarioRemetenteMestreAtivoDaConta(ContaUsuario? usuario)
        {
            ValidaUsuarioPertenceConta(usuario);
            ValidaUsuarioMestreAtivo(usuario!);
        }
        private void ValidaUsuarioPertenceConta(ContaUsuario? usuario)
        {
            ContasUsuariosValidacao.Verifica(usuario is null || usuario.Conta != Conta, MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        }
        private void ValidaUsuarioMestreAtivo(ContaUsuario usuario)
        {
            ContasUsuariosValidacao.Verifica(usuario.Acesso != TiposAcessos.Mestre, MensagensContasUsuarios.ACESSO_NEGADO);
            ContasUsuariosValidacao.Verifica(usuario.Status != StatusContasUsuario.Ativo, MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
        }
        #endregion



    }
}
