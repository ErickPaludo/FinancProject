using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesConvite
    {
        private Conta CriarContaAtiva() => new Conta(1, "Conta Teste", "#FFFFFF");

        private Usuario CriarUsuario(string id = "user-123")
            => new Usuario(id, "Nome", "Sobrenome", $"{id}@teste.com", "salt", "hash");

        private ContaUsuario CriarMestreAtivo(Conta conta, string idUsuario = "mestre-1")
        {
            var usuario = new ContaUsuario(conta, idUsuario);
            conta.AddUsuario(usuario);
            return usuario;
        }

        #region Construtor

        [Fact]
        public void Convite_DeveCriarComSucesso_QuandoDadosForemValidos()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var remetente = CriarMestreAtivo(conta);
            var destinatario = CriarUsuario("dest-1");

            // Act
            var convite = new Convite(TiposAcessos.Administrador, remetente, destinatario);

            // Assert
            convite.Acesso.Should().Be(TiposAcessos.Administrador);
            convite.IdUsuarioRemetente.Should().Be(remetente.IdUsuario);
            convite.IdUsuarioDestinatario.Should().Be(destinatario.Id);
            convite.Expiracao.Should().BeAfter(DateTime.UtcNow);
            convite.Aceito.Should().BeNull();
        }

        [Fact]
        public void Convite_DeveLancarExcecao_QuandoRemetenteNaoForMestre()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var remetenteComum = new ContaUsuario(1, conta, "admin", TiposAcessos.Administrador, StatusContasUsuario.Ativo);
            conta.AddUsuario(remetenteComum);
            var destinatario = CriarUsuario("dest-1");

            // Act
            Action action = () => new Convite(TiposAcessos.Administrador, remetenteComum, destinatario);

            // Assert
            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.USUARIO_SEM_PERMISSAO);
        }

        [Fact]
        public void Convite_DeveLancarExcecao_QuandoDestinatarioJaPertenceAConta()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var remetente = CriarMestreAtivo(conta, "mestre");
            var destinatario = CriarUsuario("dest-1");
            var contaUsuarioDest = new ContaUsuario(conta, destinatario.Id);
            conta.AddUsuario(contaUsuarioDest);

            // Act
            Action action = () => new Convite(TiposAcessos.Administrador, remetente, destinatario);

            // Assert
            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.USUARIO_JA_PERTENCE_A_CONTA);
        }

        [Fact]
        public void Convite_DeveLancarExcecao_QuandoJaExisteConviteEmAndamento()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var remetente = CriarMestreAtivo(conta);
            var destinatario = CriarUsuario("dest-1");

            // Primeiro convite
            var convite1 = new Convite(TiposAcessos.Visualizador, remetente, destinatario);
            conta.AddConvite(convite1);

            // Act
            Action action = () => new Convite(TiposAcessos.Administrador, remetente, destinatario);

            // Assert
            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_EM_ANDAMENTO);
        }

        [Fact]
        public void Convite_DeveLancarExcecao_QuandoAtingirLimiteDeMestres()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var mestre1 = CriarMestreAtivo(conta, "mestre-1");
            var mestre2 = CriarMestreAtivo(conta, "mestre-2");
            var destinatario = CriarUsuario("dest-1");

            // Act
            Action action = () => new Convite(TiposAcessos.Mestre, mestre1, destinatario);

            // Assert
            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensBase.LIMITE_USUARIOS_MESTRES);
        }

        #endregion

        #region Acoes do Convite

        [Fact]
        public void AceitaConvite_DeveAlterarStatus_QuandoPendente()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var remetente = CriarMestreAtivo(conta);
            var destinatario = CriarUsuario("dest-1");
            var convite = new Convite(TiposAcessos.Administrador, remetente, destinatario);

            // Act
            convite.AceitaConvite(true);

            // Assert
            convite.Aceito.Should().BeTrue();
        }

        [Fact]
        public void AceitaConvite_DeveLancarExcecao_QuandoJaVisualizado()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var remetente = CriarMestreAtivo(conta);
            var destinatario = CriarUsuario("dest-1");
            var convite = new Convite(TiposAcessos.Administrador, remetente, destinatario);
            convite.AceitaConvite(true);

            // Act
            Action action = () => convite.AceitaConvite(false);

            // Assert
            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_JA_VISUALIZADO + "aceito");
        }

        [Fact]
        public void RevogaConvite_DeveValidarRemetente()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var remetente = CriarMestreAtivo(conta, "mestre-original");
            var destinatario = CriarUsuario("dest-1");
            var convite = new Convite(TiposAcessos.Administrador, remetente, destinatario);

            // Act
            Action action = () => convite.RevogaConvite("outro-usuario");

            // Assert
            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_EXPIRADO); // O sistema usa a mesma mensagem para remetente inválido na revogação
        }

        #endregion

        #region Observacoes

        [Fact]
        public void InsereObservacao_DeveAtualizarPropriedade()
        {
            // Arrange
            var conta = CriarContaAtiva();
            var remetente = CriarMestreAtivo(conta);
            var destinatario = CriarUsuario("dest-1");
            var convite = new Convite(TiposAcessos.Administrador, remetente, destinatario);
            var obs = "Bem-vindo à conta!";

            // Act
            convite.InsereObservacao(obs);

            // Assert
            convite.Observacao.Should().Be(obs);
        }

        #endregion
    }
}
