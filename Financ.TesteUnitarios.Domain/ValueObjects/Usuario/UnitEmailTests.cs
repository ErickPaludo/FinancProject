using System;
using FluentAssertions;
using Xunit;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Usuarios;
using Financ.Domain.Validacoes.Usuarios.Mensagens;

namespace Financ.Domain.Tests.Objetos_de_Valor
{
    public class UnitEmailTests
    {
        // ---------------------------------------------------------------
        // Helpers de massa de dados (evita "números mágicos" nos testes)
        // ---------------------------------------------------------------

        // Gera um endereço de e-mail sintaticamente válido com exatamente
        // 'tamanhoTotal' caracteres, usando o domínio fixo "a@b.br" (6 chars).
        private static string CriarEmailComTamanhoExato(int tamanhoTotal)
        {
            const string dominio = "a@b.br"; // 6 caracteres
            var tamanhoLocal = tamanhoTotal - dominio.Length;
            var parteLocal = new string('a', tamanhoLocal);
            return parteLocal + dominio;
        }

        // ---------------------------------------------------------------
        // Create - Cenários válidos
        // ---------------------------------------------------------------

        [Fact]
        public void Create_EnderecoValido_DeveCriarEmailComEnderecoInformado()
        {
            // Arrange
            const string enderecoValido = "usuario@dominio.com";

            // Act
            var email = Email.Create(enderecoValido);

            // Assert
            email.Endereco.Should().Be(enderecoValido);
        }

        [Fact]
        public void Create_EnderecoComEspacosNasExtremidades_DeveRemoverEspacosAntesDeArmazenar()
        {
            // Arrange
            const string enderecoComEspacos = "   usuario@dominio.com   ";
            const string enderecoEsperado = "usuario@dominio.com";

            // Act
            var email = Email.Create(enderecoComEspacos);

            // Assert
            email.Endereco.Should().Be(enderecoEsperado);
        }

        [Theory]
        [InlineData("a@b.co")]                 // 6 caracteres - limite mínimo exato
        [InlineData("usuario@dominio.com")]
        [InlineData("usuario.nome@dominio.com.br")]
        [InlineData("usuario+tag@dominio.com")]
        [InlineData("usuario_nome@sub.dominio.com")]
        public void Create_EnderecoComFormatoValido_DeveCriarComSucesso(string enderecoValido)
        {
            // Arrange
            // (endereço fornecido via Theory)

            // Act
            var acao = () => Email.Create(enderecoValido);

            // Assert
            acao.Should().NotThrow();
        }

        [Fact]
        public void Create_EnderecoComTamanhoMinimoExato_DeveCriarComSucesso()
        {
            // Arrange
            var enderecoLimiteMinimo = CriarEmailComTamanhoExato(6);

            // Act
            var email = Email.Create(enderecoLimiteMinimo);

            // Assert
            email.Endereco.Should().HaveLength(6);
        }

        [Fact]
        public void Create_EnderecoComTamanhoMaximoExato_DeveCriarComSucesso()
        {
            // Arrange
            var enderecoLimiteMaximo = CriarEmailComTamanhoExato(256);

            // Act
            var email = Email.Create(enderecoLimiteMaximo);

            // Assert
            email.Endereco.Should().HaveLength(256);
        }

        // ---------------------------------------------------------------
        // Create - Obrigatoriedade
        // ---------------------------------------------------------------

        [Fact]
        public void Create_EnderecoNulo_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            string enderecoNulo = null;

            // Act
            var acao = () => Email.Create(enderecoNulo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.EMAIL_OBRIGATORIO);
        }

        [Fact]
        public void Create_EnderecoVazio_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            const string enderecoVazio = "";

            // Act
            var acao = () => Email.Create(enderecoVazio);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.EMAIL_OBRIGATORIO);
        }

        [Fact]
        public void Create_EnderecoApenasComEspacosEmBranco_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            const string enderecoEmBranco = "     ";

            // Act
            var acao = () => Email.Create(enderecoEmBranco);

            // Assert
            // Garante que a checagem de obrigatoriedade acontece ANTES do Trim/tamanho,
            // ou seja, "   " (que teria 0 chars após Trim) deve cair na regra de obrigatório,
            // e não na regra de tamanho mínimo.
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.EMAIL_OBRIGATORIO);
        }

        // ---------------------------------------------------------------
        // Create - Tamanho mínimo
        // ---------------------------------------------------------------

        [Theory]
        [InlineData("a")]
        [InlineData("ab@c")]
        [InlineData("abcd@")]
        public void Create_EnderecoComTamanhoMenorQueMinimo_DeveLancarUsuariosValidacaoComMensagemMinimo(string enderecoCurto)
        {
            // Arrange
            // (endereço fornecido via Theory, sempre com menos de 6 caracteres)

            // Act
            var acao = () => Email.Create(enderecoCurto);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.EMAIL_MINIMO);
        }

        // ---------------------------------------------------------------
        // Create - Tamanho máximo
        // ---------------------------------------------------------------

        [Fact]
        public void Create_EnderecoComTamanhoMaiorQueMaximo_DeveLancarUsuariosValidacaoComMensagemMaximo()
        {
            // Arrange
            var enderecoAcimaDoLimite = CriarEmailComTamanhoExato(257);

            // Act
            var acao = () => Email.Create(enderecoAcimaDoLimite);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.EMAIL_MAXIMO);
        }

        // ---------------------------------------------------------------
        // Create - Formato inválido
        // ---------------------------------------------------------------

        [Theory]
        [InlineData("emailsemarroba")]
        [InlineData("email@")]
        [InlineData("@dominio.com")]
        [InlineData("email@@dominio.com")]
        [InlineData("email@dominio@com")]
        [InlineData("email@ dominio.com")]
        public void Create_EnderecoComFormatoInvalido_DeveLancarUsuariosValidacaoComMensagemInvalido(string enderecoInvalido)
        {
            // Arrange
            // (endereço fornecido via Theory, todos com >= 6 chars e <= 256 chars,
            // para garantir que a falha é realmente de FORMATO e não de tamanho)

            // Act
            var acao = () => Email.Create(enderecoInvalido);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.EMAIL_INVALIDO);
        }

        // ---------------------------------------------------------------
        // Igualdade de Value Object
        // ---------------------------------------------------------------

        [Fact]
        public void Equals_DoisEmailsComMesmoEndereco_DevemSerIguais()
        {
            // Arrange
            var email1 = Email.Create("usuario@dominio.com");
            var email2 = Email.Create("usuario@dominio.com");

            // Act
            var saoIguais = email1.Equals(email2);

            // Assert
            saoIguais.Should().BeTrue();
            (email1 == email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_DoisEmailsComEnderecosDiferentes_NaoDevemSerIguais()
        {
            // Arrange
            var email1 = Email.Create("usuario1@dominio.com");
            var email2 = Email.Create("usuario2@dominio.com");

            // Act
            var saoIguais = email1.Equals(email2);

            // Assert
            saoIguais.Should().BeFalse();
            (email1 != email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_DoisEmailsComMesmoEnderecoEmCaixaDiferente_NaoDevemSerIguais()
        {
            // Arrange
            // Documenta a ausência de normalização de caixa (ver observações de modelagem):
            // o VO trata "Usuario@Dominio.com" e "usuario@dominio.com" como valores distintos.
            var email1 = Email.Create("Usuario@Dominio.com");
            var email2 = Email.Create("usuario@dominio.com");

            // Act
            var saoIguais = email1.Equals(email2);

            // Assert
            saoIguais.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_DoisEmailsComMesmoEndereco_DevemTerMesmoHashCode()
        {
            // Arrange
            var email1 = Email.Create("usuario@dominio.com");
            var email2 = Email.Create("usuario@dominio.com");

            // Act
            var hash1 = email1.GetHashCode();
            var hash2 = email2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void Create_MesmoEnderecoEmChamadasDiferentes_DeveGerarInstanciasDistintasPoremIguaisPorValor()
        {
            // Arrange
            var email1 = Email.Create("usuario@dominio.com");
            var email2 = Email.Create("usuario@dominio.com");

            // Act
            var saoAMesmaReferencia = ReferenceEquals(email1, email2);
            var saoIguaisPorValor = email1.Equals(email2);

            // Assert
            saoAMesmaReferencia.Should().BeFalse();
            saoIguaisPorValor.Should().BeTrue();
        }
    }
}