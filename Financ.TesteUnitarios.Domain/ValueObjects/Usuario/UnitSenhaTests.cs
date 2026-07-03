using System;
using FluentAssertions;
using Xunit;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Usuarios;
using Financ.Domain.Validacoes.Usuarios.Mensagens;

namespace Financ.Domain.Tests.Objetos_de_Valor
{
    public class UnitSenhaTests
    {
        // ---------------------------------------------------------------
        // Create - Cenários válidos
        // ---------------------------------------------------------------

        [Fact]
        public void Create_SaltEHashValidos_DeveCriarSenhaComValoresInformados()
        {
            // Arrange
            const string salt = "salt123";
            const string hash = "hash456";

            // Act
            var senha = Senha.Create(salt, hash);

            // Assert
            senha.Salt.Should().Be(salt);
            senha.Hash.Should().Be(hash);
        }

        [Fact]
        public void Create_SaltComEspacosNasExtremidades_DeveRemoverEspacosAntesDeArmazenar()
        {
            // Arrange
            const string saltComEspacos = "   salt123   ";
            const string hash = "hash456";

            // Act
            var senha = Senha.Create(saltComEspacos, hash);

            // Assert
            senha.Salt.Should().Be("salt123");
        }

        [Fact]
        public void Create_HashComEspacosNasExtremidades_DeveRemoverEspacosAntesDeArmazenar()
        {
            // Arrange
            const string salt = "salt123";
            const string hashComEspacos = "   hash456   ";

            // Act
            var senha = Senha.Create(salt, hashComEspacos);

            // Assert
            senha.Hash.Should().Be("hash456");
        }

        [Theory]
        [InlineData("a")]                                      // 1 caractere - não há regra de tamanho mínimo
        [InlineData("N2VmMjM0NTY3ODkwYWJjZGVm")]                 // formato base64-like plausível
        [InlineData("5f4dcc3b5aa765d61d8327deb882cf99")]         // formato hex-like plausível (md5)
        [InlineData("salt-com-simbolos_123!@#$%")]               // símbolos diversos
        public void Create_SaltComQualquerConteudoNaoVazio_DeveCriarComSucesso(string saltValido)
        {
            // Arrange
            const string hash = "hash456";

            // Act
            var acao = () => Senha.Create(saltValido, hash);

            // Assert
            // Documenta que não existe regra de tamanho/formato para Salt/Hash:
            // qualquer conteúdo não vazio é aceito.
            acao.Should().NotThrow();
        }

        [Fact]
        public void Create_SaltEHashComConteudoMuitoLongo_DeveCriarComSucesso()
        {
            // Arrange
            // Documenta que não existe regra de tamanho máximo para Salt/Hash.
            var saltLongo = new string('a', 1000);
            var hashLongo = new string('b', 1000);

            // Act
            var acao = () => Senha.Create(saltLongo, hashLongo);

            // Assert
            acao.Should().NotThrow();
        }

        // ---------------------------------------------------------------
        // Create - Obrigatoriedade do Salt
        // ---------------------------------------------------------------

        [Fact]
        public void Create_SaltNulo_DeveLancarUsuariosValidacaoComMensagemSenhaVazia()
        {
            // Arrange
            string saltNulo = null;
            const string hash = "hash456";

            // Act
            var acao = () => Senha.Create(saltNulo, hash);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.SENHA_VAZIA);
        }

        [Fact]
        public void Create_SaltVazio_DeveLancarUsuariosValidacaoComMensagemSenhaVazia()
        {
            // Arrange
            const string saltVazio = "";
            const string hash = "hash456";

            // Act
            var acao = () => Senha.Create(saltVazio, hash);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.SENHA_VAZIA);
        }

        [Fact]
        public void Create_SaltApenasEspacos_DeveLancarUsuariosValidacaoComMensagemSenhaVazia()
        {
            // Arrange
            const string saltEmBranco = "     ";
            const string hash = "hash456";

            // Act
            var acao = () => Senha.Create(saltEmBranco, hash);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.SENHA_VAZIA);
        }

        // ---------------------------------------------------------------
        // Create - Obrigatoriedade do Hash
        // ---------------------------------------------------------------

        [Fact]
        public void Create_HashNulo_DeveLancarUsuariosValidacaoComMensagemSenhaVazia()
        {
            // Arrange
            const string salt = "salt123";
            string hashNulo = null;

            // Act
            var acao = () => Senha.Create(salt, hashNulo);

            // Assert
            // Comprova que Hash é validado de forma independente do Salt
            // (o Salt aqui é válido, então a falha só pode vir do Hash).
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.SENHA_VAZIA);
        }

        [Fact]
        public void Create_HashVazio_DeveLancarUsuariosValidacaoComMensagemSenhaVazia()
        {
            // Arrange
            const string salt = "salt123";
            const string hashVazio = "";

            // Act
            var acao = () => Senha.Create(salt, hashVazio);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.SENHA_VAZIA);
        }

        [Fact]
        public void Create_HashApenasEspacos_DeveLancarUsuariosValidacaoComMensagemSenhaVazia()
        {
            // Arrange
            const string salt = "salt123";
            const string hashEmBranco = "     ";

            // Act
            var acao = () => Senha.Create(salt, hashEmBranco);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.SENHA_VAZIA);
        }

        // ---------------------------------------------------------------
        // Igualdade de Value Object
        // ---------------------------------------------------------------
        // Relevante não só como boa prática de VO: Usuario.AtualizaSenha() depende
        // de Senha == senha para impedir a reutilização da mesma senha, então a
        // corretude da igualdade aqui tem impacto direto numa regra de negócio.

        [Fact]
        public void Equals_MesmoSaltEHash_DevemSerIguais()
        {
            // Arrange
            var senha1 = Senha.Create("salt123", "hash456");
            var senha2 = Senha.Create("salt123", "hash456");

            // Act
            var saoIguais = senha1.Equals(senha2);

            // Assert
            saoIguais.Should().BeTrue();
            (senha1 == senha2).Should().BeTrue();
        }

        [Fact]
        public void Equals_SaltDiferente_NaoDevemSerIguais()
        {
            // Arrange
            var senha1 = Senha.Create("saltA", "hash456");
            var senha2 = Senha.Create("saltB", "hash456");

            // Act
            var saoIguais = senha1.Equals(senha2);

            // Assert
            saoIguais.Should().BeFalse();
            (senha1 != senha2).Should().BeTrue();
        }

        [Fact]
        public void Equals_HashDiferente_NaoDevemSerIguais()
        {
            // Arrange
            var senha1 = Senha.Create("salt123", "hashA");
            var senha2 = Senha.Create("salt123", "hashB");

            // Act
            var saoIguais = senha1.Equals(senha2);

            // Assert
            saoIguais.Should().BeFalse();
        }

        [Fact]
        public void Equals_MesmoValorEmCaixaDiferente_NaoDevemSerIguais()
        {
            // Arrange
            // Diferente de Email/Nome, aqui a ausência de normalização de caixa é
            // o comportamento CORRETO esperado: hash e salt são case-sensitive por
            // natureza (base64/hex diferenciam maiúsculas de minúsculas).
            var senha1 = Senha.Create("Salt123", "Hash456");
            var senha2 = Senha.Create("salt123", "hash456");

            // Act
            var saoIguais = senha1.Equals(senha2);

            // Assert
            saoIguais.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_SenhasIguais_DevemTerMesmoHashCode()
        {
            // Arrange
            var senha1 = Senha.Create("salt123", "hash456");
            var senha2 = Senha.Create("salt123", "hash456");

            // Act
            var hash1 = senha1.GetHashCode();
            var hash2 = senha2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void Create_MesmosValoresEmInstanciasDiferentes_DevemSerIguaisPorValorMasNaoPorReferencia()
        {
            // Arrange
            var senha1 = Senha.Create("salt123", "hash456");
            var senha2 = Senha.Create("salt123", "hash456");

            // Act
            var saoAMesmaReferencia = ReferenceEquals(senha1, senha2);
            var saoIguaisPorValor = senha1.Equals(senha2);

            // Assert
            saoAMesmaReferencia.Should().BeFalse();
            saoIguaisPorValor.Should().BeTrue();
        }
    }
}