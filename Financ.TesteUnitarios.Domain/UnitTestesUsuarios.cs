using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.Usuarios;
using Financ.Domain.Validacoes.Usuarios.Mensagens;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesUsuarios
    {
        private Usuario CriarUsuarioValido()
        {
            return new Usuario(
                Guid.NewGuid().ToString(),
                "Joao",
                "Silva",
                "joao.silva@teste.com",
                "salt123",
                "hash123");
        }

        #region Construtores

        [Fact]
        public void Usuario_DeveCriarComSucesso_QuandoDadosForemValidos()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var primeiroNome = "Erick";
            var segundoNome = "Paludo";
            var email = "erick@teste.com";

            // Act
            var usuario = new Usuario(id, primeiroNome, segundoNome, email, "salt", "hash");

            // Assert
            usuario.Id.Should().Be(id);
            usuario.PrimeiroNome.Should().Be(primeiroNome);
            usuario.SegundoNome.Should().Be(segundoNome);
            usuario.Email.Should().Be(email);
            usuario.NomeCompleto.Should().Be($"{primeiroNome} {segundoNome}");
        }

        [Fact]
        public void Usuario_DeveLancarExcecao_QuandoIdNaoForInformado()
        {
            // Act
            Action action = () => new Usuario("", "Joao", "Silva", "teste@teste.com", "salt", "hash");

            // Assert
            action.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensBase.USUARIO_NAO_INFORMADO);
        }

        [Theory]
        [InlineData("Jo")] // Menor que 3
        [InlineData("")]   // Vazio
        public void Usuario_DeveLancarExcecao_QuandoPrimeiroNomeForCurtoOuVazio(string nomeInvalido)
        {
            // Act
            Action action = () => new Usuario(Guid.NewGuid().ToString(), nomeInvalido, "Silva", "teste@teste.com", "salt", "hash");

            // Assert
            // Pode lançar PRIMEIRO_NOME_OBRIGATORIO ou PRIMEIRO_NOME_MINIMO dependendo do caso
            action.Should().Throw<UsuariosValidacao>();
        }

        [Theory]
        [InlineData(" Joao")]   // Espaço no início
        [InlineData("Joao ")]   // Espaço no fim
        [InlineData("Jo  ao")]  // Espaço duplo
        public void Usuario_DeveLancarExcecao_QuandoPrimeiroNomeTiverEspacosInvalidos(string nomeInvalido)
        {
            // Act
            Action action = () => new Usuario(Guid.NewGuid().ToString(), nomeInvalido, "Silva", "teste@teste.com", "salt", "hash");

            // Assert
            action.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.PRIMEIRO_NOME_INVALIDO);
        }

        [Theory]
        [InlineData(" Silva")]   // Espaço no início
        [InlineData("Silva ")]   // Espaço no fim
        [InlineData("Sil  va")]  // Espaço duplo
        public void Usuario_DeveLancarExcecao_QuandoSegundoNomeTiverEspacosInvalidos(string nomeInvalido)
        {
            // Act
            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao", nomeInvalido, "teste@teste.com", "salt", "hash");

            // Assert
            action.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.SEGUNDO_NOME_INVALIDO);
        }

        [Fact]
        public void Usuario_DeveLancarExcecao_QuandoNomeContiverCaracteresEspeciaisNaoPermitidos()
        {
            // Act
            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao123", "Silva!", "teste@teste.com", "salt", "hash");

            // Assert
            action.Should().Throw<UsuariosValidacao>();
        }

        #endregion

        #region Senha e Seguranca

        [Fact]
        public void AtualizaSenha_DeveAlterarDados_QuandoNovosDadosForemDiferentes()
        {
            // Arrange
            var usuario = CriarUsuarioValido();
            var novoSalt = "novo_salt";
            var novoHash = "novo_hash";

            // Act
            usuario.AtualizaSenha(novoSalt, novoHash);

            // Assert
            usuario.Salt.Should().Be(novoSalt);
            usuario.HashPass.Should().Be(novoHash);
        }

        [Fact]
        public void AtualizaSenha_DeveLancarExcecao_QuandoSaltForIgualAoAnterior()
        {
            // Arrange
            var usuario = CriarUsuarioValido();

            // Act
            Action action = () => usuario.AtualizaSenha(usuario.Salt, "novo_hash");

            // Assert
            action.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.MESMA_SENHA);
        }

        #endregion
    }
}
