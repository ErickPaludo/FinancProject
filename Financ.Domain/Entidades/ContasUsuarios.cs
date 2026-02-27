using Financ.Domain.Enums;
using Financ.Domain.Interfaces.InterfaceEntidades;
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
        public Conta? Contas { get; private set; }
        
        public ContasUsuarios() { }
        public ContasUsuarios(int id,Conta conta, string idUsuario, TiposAcessos acesso, TiposStatusContas? status = null)
        {
            ContasUsuariosValidacao.Verifica(id <= 0, MensagensBase.ID_IGUAL_MENOR_ZERO);
            Id = id;
            ValidaContasUsuarios(conta, idUsuario);
            ValidaEnums(acesso, status);
        }
        public ContasUsuarios(Conta conta, string idUsuario, TiposAcessos acesso)
        {
            ValidaContasUsuarios(conta, idUsuario);
            ValidaEnums(acesso, null);
        }

        public ContasUsuarios(Conta conta, Usuario usuario)
        {
            ValidaContasUsuarios(conta, usuario.IdUsuario);
            Status = TiposStatusContas.Ativo;
            Acesso = TiposAcessos.Mestre;
        }
        private void ValidaEnums(TiposAcessos acesso, TiposStatusContas? status)
        {
            Acesso = acesso;
            if (status.HasValue)
            {
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(TiposStatusContas), status), MensagensBase.STATUS_INVALIDO);
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(TiposAcessos), acesso), MensagensContasUsuarios.ACESSO_INVALIDO);
                Status = status.Value;
            }
            else
                Status = TiposStatusContas.Ativo;
        }
        private void ValidaContasUsuarios(Conta conta, string idUsuario)
        {
            ContasUsuariosValidacao.Verifica(conta is null, MensagensContasUsuarios.CONTA_NAO_PODE_SER_NULA);
            ContasUsuariosValidacao.Verifica(conta!.Status != TiposStatusContas.Ativo, MensagensContasUsuarios.CONTA_NAO_ESTA_ATIVA);
            ContasUsuariosValidacao.Verifica(string.IsNullOrEmpty(idUsuario), MensagensContasUsuarios.IDUSUARIO_INVALIDO);
            Contas = conta;
            //Usuario = usuario!;
            IdUsuario = idUsuario;
            DthrReg = DateTime.Now;
        }
        public void AtualizaOutraContaUsuario(ContasUsuarios contasUsuarioRemetente,TiposAcessos? acessos, TiposStatusContas? status)
        {
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente.IdUsuario == IdUsuario, MensagensContasUsuarios.ACESSO_NEGADO);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente.Acesso != TiposAcessos.Mestre, MensagensContasUsuarios.ACESSO_NEGADO);
            ContasUsuariosValidacao.Verifica(contasUsuarioRemetente.Status != TiposStatusContas.Ativo, MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
            ContasUsuariosValidacao.Verifica(Acesso == TiposAcessos.Mestre, MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO);


            if (acessos.HasValue)
            {
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(TiposAcessos), acessos.Value), MensagensContasUsuarios.ACESSO_INVALIDO);
                Acesso = acessos.Value;
            }
            if (status.HasValue)
            {
                ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(TiposStatusContas), status.Value), MensagensBase.STATUS_INVALIDO);
                Status = status.Value;
            }
        }
        

    }
}
