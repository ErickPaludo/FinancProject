using Financ.Domain.Enums;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades
{
    public sealed class ContasUsuarios : BaseConta
    {
        public int IdConta { get; private set; }
        public string IdUsuario { get; private set; }
        public TiposAcessos Acesso { get; private set; }
        public TipoStatusContasUsuario Status { get; protected set; }

        public Usuario Usuario { get; private set; }

        public Conta Conta { get; private set; }


        public ContasUsuarios() { }
        public ContasUsuarios(int id, Conta conta, string idUsuario, TiposAcessos acesso, TipoStatusContasUsuario? status = null)
        {
            ContasUsuariosValidacao.Verifica(id <= 0, MensagensBase.ID_IGUAL_MENOR_ZERO);
            Id = id;
            ValidaContasUsuarios(conta, idUsuario);
            ValidaEnums(acesso, status);
            Status = status.HasValue ? status.Value : TipoStatusContasUsuario.Ativo;
            Acesso = acesso;

        }
        public ContasUsuarios(Convites convite)
        {
            ValidaContasUsuarios(convite.Conta, convite.IdUsuarioDestinatario);
            ValidaEnums(convite.Acesso, null);

            if (!ValidaPermissoeNaConta(convite.Acesso))
            {
                Acesso = TiposAcessos.Administrador;
                convite.InsereObservacao($"{MensagensConvite.CONTA_JA_POSSUI_UM_USUARIO_MESTRES} {MensagensContasUsuarios.MAX_MESTRES_CONVERTE_PARA_ADMIN}");
            }
            else
            {
                Acesso = convite.Acesso;
            }

            Status = TipoStatusContasUsuario.Ativo;

        }

        public ContasUsuarios(Conta conta, string idUasuario)
        {
            ValidaContasUsuarios(conta, idUasuario);
            Status = TipoStatusContasUsuario.Ativo;
            Acesso = TiposAcessos.Mestre;
        }
        private void ValidaEnums(TiposAcessos? acesso, TipoStatusContasUsuario? status)
        {

            if (status.HasValue)
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(TipoStatusContasUsuario), status), MensagensBase.STATUS_INVALIDO);
           
            if (acesso.HasValue)
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(TiposAcessos), acesso), MensagensContasUsuarios.ACESSO_INVALIDO);
        }
        private void ValidaContasUsuarios(Conta conta, string idUsuario)
        {
            ContasUsuariosValidacao.Verifica(conta is null, MensagensContasUsuarios.CONTA_NAO_PODE_SER_NULA);
            ContasUsuariosValidacao.Verifica(conta!.Status != TiposStatusContas.Ativo, MensagensContasUsuarios.CONTA_NAO_ESTA_ATIVA);
            ContasUsuariosValidacao.Verifica(string.IsNullOrEmpty(idUsuario), MensagensContasUsuarios.IDUSUARIO_INVALIDO);
            Conta = conta;
            //Usuario = usuario!;
            IdUsuario = idUsuario;
            DthrReg = DateTime.Now;
        }
        public void AtualizaOutraContaUsuario(ContasUsuarios? contasUsuarioRemetente, TiposAcessos? acesso, TipoStatusContasUsuario? status)
        {
            ValidaUsuarioRemetenteMestreAtivoDaConta(contasUsuarioRemetente);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente == this, MensagensContasUsuarios.USUARIO_TENTA_SE_ATUALIZAR);
            ContasUsuariosValidacao.Verifica(Acesso == TiposAcessos.Mestre, MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO);

            ValidaEnums(acesso, status);

            if (acesso.HasValue && status.HasValue)
                ContasUsuariosValidacao.Verifica(acesso.Value.Equals(TiposAcessos.Mestre) && !status.Value.Equals(TipoStatusContasUsuario.Ativo), MensagensContasUsuarios.ATUALIZA_PARA_USUARIO_MESTRE_DIFERENTE_DE_ATIVO);

            if (acesso.HasValue)
            {
                ContasUsuariosValidacao.Verifica(!ValidaPermissoeNaConta(acesso.Value), MensagensBase.LIMITE_USUARIOS_MESTRES);
                Acesso = acesso.Value;
            }
            if (status.HasValue)
            {
                Status = status.Value;
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

            //Verifica se a conta possui mais usuários conectados a conta, e se o usuário é o único mestre, para evitar que a conta fique sem um usuário mestre.

            ContasUsuariosValidacao.Verifica(Conta.Convites.Any(x => DateTime.Now <= x.Expiracao && x.Aceito is null && x.IdUsuarioRemetente.Equals(IdUsuario)), MensagensContasUsuarios.USUARIO_POSSUI_CONVITES_EM_ANDAMENTO);

        }
        public void RemoverUsuarioDaConta(ContasUsuarios? contasUsuarioRemetente)
        {
            ValidaUsuarioRemetenteMestreAtivoDaConta(contasUsuarioRemetente);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente == this, MensagensContasUsuarios.USUARIO_TENTA_SE_EXPULSAR);
            ContasUsuariosValidacao.Verifica(Acesso == TiposAcessos.Mestre, MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_REMOVIDO);
        }
        private void ValidaUsuarioRemetenteMestreAtivoDaConta(ContasUsuarios? usuario)
        {
            ValidaUsuarioPertenceConta(usuario);
            ValidaUsuarioMestreAtivo(usuario!);
        }

        private void ValidaUsuarioPertenceConta(ContasUsuarios? usuario)
        {
            ContasUsuariosValidacao.Verifica(
                usuario is null || usuario.Conta != Conta,
                MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        }

        private void ValidaUsuarioMestreAtivo(ContasUsuarios usuario)
        {
            ContasUsuariosValidacao.Verifica(
                usuario.Acesso != TiposAcessos.Mestre,
                MensagensContasUsuarios.ACESSO_NEGADO);

            ContasUsuariosValidacao.Verifica(
                usuario.Status != TipoStatusContasUsuario.Ativo,
                MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
        }
        public bool ValidaPermissoeNaConta(TiposAcessos acessoDestinatario)
        {
            return !(acessoDestinatario.Equals(TiposAcessos.Mestre) && Conta.ContaUsuarios.Where(x => x.Acesso.Equals(TiposAcessos.Mestre) && x.Status.Equals(TipoStatusContasUsuario.Ativo)).Take(2).Count() == 2);
        }

    }
}
