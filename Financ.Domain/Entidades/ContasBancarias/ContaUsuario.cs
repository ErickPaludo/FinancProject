using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Objetos_de_Valor.ContaUsuario;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;

namespace Financ.Domain.Entidades.ContasBancarias
{
    public sealed class ContaUsuario : EntidadeBase
    {
        public Usuario Usuario { get; }
        public ContaBancaria ContaBancaria { get; }
        public ETiposAcessos Acesso { get; private set; }
        public EStatusContasUsuario Status { get; private set; } = EStatusContasUsuario.Ativo;
        public ExpiracaoContaUsuario? Expiracao { get; private set; }
        public PreferenciasContaUsuario Preferencias { get; } = PreferenciasContaUsuario.Create();

        private ContaUsuario(ContaBancaria conta, Usuario usuario,ETiposAcessos acessos)
        {
            ValidaNulo.Verifica(conta, MensagensBase.CONTA_NULA);
            ValidaNulo.Verifica(usuario, MensagensBase.USUARIO_NULO);
            Acesso = acessos;
            ContaBancaria = conta;
            Usuario = usuario;
        }

        public static ContaUsuario CriarPorConvite(Convite convite)
        {
            return new ContaUsuario(convite.ContaBancaria, convite.Destinatario, convite.Acesso);
        }
        public static ContaUsuario CriarPrimeiroUsuario(ContaBancaria conta, Usuario usuario)
        {
            return new ContaUsuario(conta, usuario, ETiposAcessos.Mestre);
        }

        //public void AtualizaOutraContaUsuario(ContaUsuario? contasUsuarioRemetente, ETiposAcessos? acesso, EStatusContasUsuario? status, ExpiracaoContaUsuario? expiracao = null, bool? removerExpiracao = null)
        //{
        //    ContasUsuariosValidacao.Verifica(contasUsuarioRemetente == this, MensagensContaUsuario.USUARIO_TENTA_SE_ATUALIZAR);
        //    ContasUsuariosValidacao.Verifica(ValidaUsuarioMestre(Acesso), MensagensContaUsuario.USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO);

        //    if (acesso.HasValue)
        //    {
        //        ValidaAcesso(acesso.Value);
        //        ContasUsuariosValidacao.Verifica(
        //           (Expiracao is not null ||
        //            (expiracao is not null)) &&
        //            ValidaUsuarioMestre(acesso.Value),
        //            MensagensContaUsuario.USUARIO_MESTRE_COM_TEMPO_LIMITE_JA_DEFINIDO);

        //        ContasUsuariosValidacao.Verifica(!ValidaPermissoeNaConta(acesso.Value), MensagensBase.LIMITE_USUARIOS_MESTRES);
        //        Acesso = acesso.Value;

        //    }

        //    if (status.HasValue)
        //    {
        //        ContasUsuariosValidacao.Verifica(!status.Value.Equals(EStatusContasUsuario.Ativo) && ValidaUsuarioMestre(Acesso), MensagensContaUsuario.ATUALIZA_PARA_USUARIO_MESTRE_DIFERENTE_DE_ATIVO);
        //        Status = status.Value;
        //    }

        //    if (expiracao is not null)
        //    {
        //        ContasUsuariosValidacao.Verifica(expiracao.EstaExpirado(), MensagensContaUsuario.TEMPO_MIN_EXPIRACAO);
        //        Expiracao = expiracao;
        //    }

        //    if (removerExpiracao.HasValue)
        //    {
        //        ContasUsuariosValidacao.Verifica(expiracao is not null && (removerExpiracao.Value), MensagensContaUsuario.CONFLITO_AO_EXPIRAR);
        //        Expiracao = removerExpiracao.Value ? null : Expiracao;
        //    }

        //}
        //public void SairDaConta()
        //{
        //    ContasUsuariosValidacao.Verifica(
        //        Acesso.Equals(ETiposAcessos.Mestre)
        //        && ContaBancaria.ContaUsuarios.Any(x => !x.Acesso.Equals(ETiposAcessos.Mestre)
        //        && !x.IdUsuario.Equals(IdUsuario))
        //        && ContaBancaria.ContaUsuarios.Where(x => x.Acesso.Equals(ETiposAcessos.Mestre)).Take(2).Count() == 1,
        //        MensagensContasUsuarios.UNICO_USUARIO_MESTRE_NA_CONTA);

        //    ContasUsuariosValidacao.Verifica(ContaBancaria.Convites.Any(x => DateTime.UtcNow <= x.Expiracao && x.Aceito is null && x.IdUsuarioRemetente.Equals(IdUsuario)), MensagensContasUsuarios.USUARIO_POSSUI_CONVITES_EM_ANDAMENTO);

        //    Status = EStatusContasUsuario.Removido;


        //}
        //public void RemoverUsuarioDaConta(ContaUsuario? contasUsuarioRemetente)
        //{
        //    ValidaUsuarioRemetenteMestreAtivoDaConta(contasUsuarioRemetente);
        //    ContasUsuariosValidacao.Verifica(contasUsuarioRemetente == this, MensagensContasUsuarios.USUARIO_TENTA_SE_EXPULSAR);
        //    ContasUsuariosValidacao.Verifica(Acesso == ETiposAcessos.Mestre, MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_REMOVIDO);
        //    Status = EStatusContasUsuario.Removido;
        //}
        //public bool ValidaPermissoeNaConta(ETiposAcessos acessoDestinatario)
        //{
        //    return !(acessoDestinatario.Equals(ETiposAcessos.Mestre) && ContaBancaria.ContaUsuarios.Where(x => x.Acesso.Equals(ETiposAcessos.Mestre) && x.Status.Equals(EStatusContasUsuario.Ativo)).Take(2).Count() == 2);
        //}
        //public bool ValidaUsuarioMestre(ETiposAcessos acesso)
        //{
        //    return ETiposAcessos.Mestre.Equals(acesso);
        //}
        //public bool ExpiracaoPorAcesso(ETiposAcessos acesso)
        //{
        //    return acesso.Equals(ETiposAcessos.Mestre);
        //}
    
        //public void ValidaSituacaoUsuarioParaConsulta()
        //{
        //    ContasUsuariosValidacao.Verifica(Expiracao < DateTime.UtcNow, MensagensContasUsuarios.USUARIO_EXPIRADO);
        //}
        //public void RetornaParaConta(Convite convite)
        //{
        //    ValidaContaBancaria(convite.Conta, convite.IdUsuarioDestinatario);
        //    ValidaStatus(convite.Acesso, null);
        //    Acesso = convite.Acesso;
        //    Status = EStatusContasUsuario.Ativo;
        //    if (convite.ExpiracaoContaUsuario.HasValue)
        //    {
        //        Expiracao = DateTime.UtcNow.AddMinutes(convite.ExpiracaoContaUsuario.Value);
        //    }
        //    else
        //    {
        //        Expiracao = null;
        //    }
        //}

        //private void ValidaStatus(EStatusContasUsuario status)
        //{
        //    ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(EStatusContasUsuario), status), MensagensBase.STATUS_INVALIDO);
        //}
        //private void ValidaAcesso(ETiposAcessos acesso)
        //{
        //    ContasUsuariosValidacao.Verifica(!Enum.IsDefined(typeof(ETiposAcessos), acesso), MensagensBase.ACESSO_INVALIDO);
        //}
        //private void ValidaContaBancaria(ContaBancaria conta)
        //{
        //    ContasUsuariosValidacao.Verifica(conta.Status != EStatusContas.Ativo, MensagensContaUsuario.CONTA_NAO_ESTA_ATIVA);
        //}
        //private void ValidaUsuarioRemetenteMestreAtivoDaConta(ContaUsuario? usuario)
        //{
        //    ValidaUsuarioPertenceConta(usuario);
        //}
        //private void ValidaUsuarioPertenceConta(ContaUsuario? usuario)
        //{
        //    ContasUsuariosValidacao.Verifica(usuario is null || usuario.ContaBancaria != ContaBancaria, MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        //}
    }
}
