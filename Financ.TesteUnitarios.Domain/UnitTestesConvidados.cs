using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using FluentAssertions;
using Xunit;
using System;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesConvites
    {
        private Conta CriarContaAtiva()
            => new Conta(1, "Conta Teste");

        private Usuario CriarUsuario(string id)
            => new Usuario(id, "Nome", "Sobrenome", $"{id}@teste.com");

        private ContasUsuarios CriarContaUsuarioMestre(Conta conta, string idUsuario,TipoStatusContasUsuario status =  TipoStatusContasUsuario.Ativo)
        {
            ContasUsuarios contaUsuario = new ContasUsuarios(
                1,
                conta,
                idUsuario,
                TiposAcessos.Mestre,status);
            conta.AddUsuario(contaUsuario);
            return contaUsuario;
        }


        private string NovoIdUsuario() => Guid.NewGuid().ToString();


        [Fact(DisplayName = "Deve criar convite quando dados são válidos")]
        public void CriarConvite_DadosValidos_NaoDeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(Guid.NewGuid().ToString());
            var usuarioDestinatario = CriarUsuario(Guid.NewGuid().ToString()).IdUsuario;
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.IdUsuario);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().NotThrow();
        }

        [Fact(DisplayName = "Não deve permitir convite se usuário não for mestre")]
        public void CriarConvite_UsuarioNaoMestre_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(Guid.NewGuid().ToString());
            var usuarioDestinatario = CriarUsuario(Guid.NewGuid().ToString()).IdUsuario;

            var contaUsuarioRemetente = new ContasUsuarios(
                1,
                conta,
                usuarioRemetente.IdUsuario,
                TiposAcessos.Administrador);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.USUARIO_SEM_PERMISSAO);
        }

        [Fact(DisplayName = "Não deve permitir convite se conta não for ativa")]
        public void CriarConvite_ContaInativa_DeveLancarExcecao()
        {
            var conta = new Conta(1, "Conta Teste");

            var usuarioRemetente = CriarUsuario(Guid.NewGuid().ToString());
            var usuarioDestinatario = CriarUsuario(Guid.NewGuid().ToString()).IdUsuario;
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.IdUsuario);

            conta.AtualizaConta(CriarContaUsuarioMestre(conta, Guid.NewGuid().ToString()), null, TiposStatusContas.Inativo); // ajuste se necessário
            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
              .WithMessage(MensagensContas.CONTA_INATIVA);
        }

        [Theory(DisplayName = "Não deve permitir convite se o  usuário remetente não for ativo")]
        [InlineData(TipoStatusContasUsuario.Inativo)]
        [InlineData(TipoStatusContasUsuario.Bloqueado)]
        public void CriarConvite_UsuarioInativo_DeveLancarExcecao(TipoStatusContasUsuario status)
        {
            var conta = new Conta(1, "Conta Teste");

            var usuarioRemetente = CriarUsuario(Guid.NewGuid().ToString());
            var usuarioDestinatario = CriarUsuario(Guid.NewGuid().ToString()).IdUsuario;
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.IdUsuario,status);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
              .WithMessage(MensagensConvite.USUARIO_CONTA_REMETENTE_INATIVO);
        }


        [Fact(DisplayName = "Aceitar convite deve funcionar quando válido")]
        public void AceitarConvite_Valido_DeveAlterarEstado()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(Guid.NewGuid().ToString());
            var usuarioDestinatario = CriarUsuario(Guid.NewGuid().ToString()).IdUsuario;
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.IdUsuario);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            convite.AceitaConvite(true);

            convite.Aceito.Should().BeTrue();
        }

        [Fact(DisplayName = "Não pode enviar convite para usuário Mestre se existir 2 usuarios mestres cadastrados.")]
        public void Limite_Mestres_Cadastrados()
        {
            var conta = CriarContaAtiva();

            var usuario1 = CriarUsuario(Guid.NewGuid().ToString()).IdUsuario;
            var usuario2 = CriarUsuario(Guid.NewGuid().ToString()).IdUsuario;
            var usuario3 = CriarUsuario(Guid.NewGuid().ToString()).IdUsuario;

            // Adiciona dois usuários mestre à conta

            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta,usuario1);
            var contaUsuarioIntermediario = CriarContaUsuarioMestre(conta, usuario2);

            Action action = () => new Convites(
                TiposAcessos.Mestre,
                contaUsuarioRemetente,
                usuario3
            );

            // Assert
            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.LIMITE_USUARIOS_MESTRES);
        }
    }
}