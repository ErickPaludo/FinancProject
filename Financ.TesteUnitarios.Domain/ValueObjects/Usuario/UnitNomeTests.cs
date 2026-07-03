using System;
using FluentAssertions;
using Xunit;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Usuarios;
using Financ.Domain.Validacoes.Usuarios.Mensagens;

namespace Financ.Domain.Tests.Objetos_de_Valor
{
    public class UnitNomeTests
    {
        // ---------------------------------------------------------------
        // Helpers de massa de dados (evita "números mágicos" nos testes)
        // ---------------------------------------------------------------

        // Gera uma string apenas com letras, com o tamanho exato solicitado.
        private static string CriarNomeComTamanhoExato(int tamanho)
        {
            return new string('a', tamanho);
        }

        // ---------------------------------------------------------------
        // Create - Cenários válidos
        // ---------------------------------------------------------------

        [Fact]
        public void Create_NomesValidos_DeveCriarNomeComPrimeiroESegundoInformados()
        {
            // Arrange
            const string primeiro = "Joao";
            const string segundo = "Silva";

            // Act
            var nome = Nome.Create(primeiro, segundo);

            // Assert
            nome.Primeiro.Should().Be(primeiro);
            nome.Segundo.Should().Be(segundo);
        }

        [Fact]
        public void Create_NomesComEspacosNasExtremidades_DeveRemoverEspacosAntesDeArmazenar()
        {
            // Arrange
            const string primeiroComEspacos = "   Joao   ";
            const string segundoComEspacos = "   Silva   ";

            // Act
            var nome = Nome.Create(primeiroComEspacos, segundoComEspacos);

            // Assert
            nome.Primeiro.Should().Be("Joao");
            nome.Segundo.Should().Be("Silva");
        }

        [Theory]
        [InlineData("Jose", "Muller")]
        [InlineData("Joao", "Concei\u00e7\u00e3o")]
        [InlineData("Ana", "Araujo")]
        public void Create_NomesComAcentuacao_DeveCriarComSucesso(string primeiro, string segundo)
        {
            // Arrange
            // (nomes fornecidos via Theory, com caracteres acentuados)

            // Act
            var acao = () => Nome.Create(primeiro, segundo);

            // Assert
            acao.Should().NotThrow();
        }

        [Fact]
        public void Create_PrimeiroNomeComEspacoInterno_DeveCriarComSucesso()
        {
            // Arrange
            // Espaço é um caractere permitido pela regra de formato, então nomes
            // compostos no campo "Primeiro" são aceitos (ex.: "Ana Maria").
            const string primeiroComposto = "Ana Maria";
            const string segundo = "Silva";

            // Act
            var nome = Nome.Create(primeiroComposto, segundo);

            // Assert
            nome.Primeiro.Should().Be(primeiroComposto);
        }

        [Fact]
        public void Create_NomeComEspacosInternosMultiplos_DeveCriarComSucessoSemNormalizar()
        {
            // Arrange
            // Documenta que espaços internos múltiplos NÃO são colapsados: a regra de
            // formato aceita qualquer sequência de letras/espaços, sem normalização.
            const string primeiroComEspacoDuplo = "Jo  se";
            const string segundo = "Silva";

            // Act
            var nome = Nome.Create(primeiroComEspacoDuplo, segundo);

            // Assert
            nome.Primeiro.Should().Be("Jo  se");
        }

        [Fact]
        public void Create_NomesComTamanhoMinimoExato_DeveCriarComSucesso()
        {
            // Arrange
            var primeiroLimiteMinimo = CriarNomeComTamanhoExato(3);
            var segundoLimiteMinimo = CriarNomeComTamanhoExato(3);

            // Act
            var nome = Nome.Create(primeiroLimiteMinimo, segundoLimiteMinimo);

            // Assert
            nome.Primeiro.Should().HaveLength(3);
            nome.Segundo.Should().HaveLength(3);
        }

        [Fact]
        public void Create_NomesComTamanhoMaximoExato_DeveCriarComSucesso()
        {
            // Arrange
            var primeiroLimiteMaximo = CriarNomeComTamanhoExato(100);
            var segundoLimiteMaximo = CriarNomeComTamanhoExato(100);

            // Act
            var nome = Nome.Create(primeiroLimiteMaximo, segundoLimiteMaximo);

            // Assert
            nome.Primeiro.Should().HaveLength(100);
            nome.Segundo.Should().HaveLength(100);
        }

        // ---------------------------------------------------------------
        // Create - Obrigatoriedade
        // ---------------------------------------------------------------

        [Fact]
        public void Create_PrimeiroNomeNulo_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            string primeiroNulo = null;
            const string segundo = "Silva";

            // Act
            var acao = () => Nome.Create(primeiroNulo, segundo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_OBRIGATORIO);
        }

        [Fact]
        public void Create_PrimeiroNomeVazio_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            const string primeiroVazio = "";
            const string segundo = "Silva";

            // Act
            var acao = () => Nome.Create(primeiroVazio, segundo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_OBRIGATORIO);
        }

        [Fact]
        public void Create_PrimeiroNomeApenasEspacos_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            const string primeiroEmBranco = "     ";
            const string segundo = "Silva";

            // Act
            var acao = () => Nome.Create(primeiroEmBranco, segundo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_OBRIGATORIO);
        }

        [Fact]
        public void Create_SegundoNomeNulo_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            const string primeiro = "Joao";
            string segundoNulo = null;

            // Act
            var acao = () => Nome.Create(primeiro, segundoNulo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_OBRIGATORIO);
        }

        [Fact]
        public void Create_SegundoNomeVazio_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            const string primeiro = "Joao";
            const string segundoVazio = "";

            // Act
            var acao = () => Nome.Create(primeiro, segundoVazio);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_OBRIGATORIO);
        }

        [Fact]
        public void Create_SegundoNomeApenasEspacos_DeveLancarUsuariosValidacaoComMensagemObrigatorio()
        {
            // Arrange
            const string primeiro = "Joao";
            const string segundoEmBranco = "     ";

            // Act
            var acao = () => Nome.Create(primeiro, segundoEmBranco);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_OBRIGATORIO);
        }

        // ---------------------------------------------------------------
        // Create - Ordem de avaliação entre os parâmetros
        // ---------------------------------------------------------------

        [Fact]
        public void Create_PrimeiroNomeInvalidoESegundoNomeInvalido_DeveValidarPrimeiroAntesDoSegundo()
        {
            // Arrange
            // Primeiro nome é nulo (violaria OBRIGATORIO) e o segundo tem caractere
            // inválido (violaria INVALIDO). Como Primeiro é preparado antes de Segundo
            // no construtor, a exceção esperada é a do Primeiro.
            string primeiroNulo = null;
            const string segundoComCaractereInvalido = "Silva1";

            // Act
            var acao = () => Nome.Create(primeiroNulo, segundoComCaractereInvalido);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_OBRIGATORIO);
        }

        // ---------------------------------------------------------------
        // Create - Formato inválido (caracteres não permitidos)
        // ---------------------------------------------------------------

        [Theory]
        [InlineData("Joao3")]
        [InlineData("Ana!")]
        [InlineData("Maria123")]
        [InlineData("O'Brien")]
        [InlineData("Anne-Marie")]
        [InlineData("Joao_Silva")]
        [InlineData("Jo@o")]
        public void Create_PrimeiroNomeComCaractereInvalido_DeveLancarUsuariosValidacaoComMensagemInvalido(string primeiroInvalido)
        {
            // Arrange
            const string segundo = "Silva";

            // Act
            var acao = () => Nome.Create(primeiroInvalido, segundo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_INVALIDO);
        }

        [Fact]
        public void Create_SegundoNomeComCaractereInvalido_DeveLancarUsuariosValidacaoComMensagemInvalido()
        {
            // Arrange
            // Confirma que a validação de formato roda de forma independente para
            // o campo Segundo, não só para o Primeiro.
            const string primeiro = "Joao";
            const string segundoInvalido = "Silva1";

            // Act
            var acao = () => Nome.Create(primeiro, segundoInvalido);

            // Assert
            // Observação de modelagem: MensagensUsuarios possui SEGUNDO_NOME_INVALIDO,
            // mas Nome.cs usa a mesma constante NOME_INVALIDO para os dois campos.
            // Este teste documenta o comportamento real, não o que "deveria" ser.
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_INVALIDO);
        }

        // ---------------------------------------------------------------
        // Create - Tamanho mínimo
        // ---------------------------------------------------------------

        [Theory]
        [InlineData("a")]
        [InlineData("ab")]
        public void Create_PrimeiroNomeComTamanhoMenorQueMinimo_DeveLancarUsuariosValidacaoComMensagemMinimo(string primeiroCurto)
        {
            // Arrange
            const string segundo = "Silva";

            // Act
            var acao = () => Nome.Create(primeiroCurto, segundo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_MINIMO);
        }

        [Fact]
        public void Create_SegundoNomeComTamanhoMenorQueMinimo_DeveLancarUsuariosValidacaoComMensagemMinimo()
        {
            // Arrange
            const string primeiro = "Joao";
            const string segundoCurto = "ab";

            // Act
            var acao = () => Nome.Create(primeiro, segundoCurto);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_MINIMO);
        }

        // ---------------------------------------------------------------
        // Create - Tamanho máximo
        // ---------------------------------------------------------------

        [Fact]
        public void Create_PrimeiroNomeComTamanhoMaiorQueMaximo_DeveLancarUsuariosValidacaoComMensagemMaximo()
        {
            // Arrange
            var primeiroAcimaDoLimite = CriarNomeComTamanhoExato(101);
            const string segundo = "Silva";

            // Act
            var acao = () => Nome.Create(primeiroAcimaDoLimite, segundo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_MAXIMO);
        }

        [Fact]
        public void Create_SegundoNomeComTamanhoMaiorQueMaximo_DeveLancarUsuariosValidacaoComMensagemMaximo()
        {
            // Arrange
            const string primeiro = "Joao";
            var segundoAcimaDoLimite = CriarNomeComTamanhoExato(101);

            // Act
            var acao = () => Nome.Create(primeiro, segundoAcimaDoLimite);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_MAXIMO);
        }

        // ---------------------------------------------------------------
        // Create - Precedência entre regras (formato roda antes de tamanho)
        // ---------------------------------------------------------------

        [Fact]
        public void Create_NomeComCaractereInvalidoEMenorQueOMinimo_DeveLancarMensagemInvalidaNaoMinimo()
        {
            // Arrange
            // "1" tem 1 caractere (< 3, violaria NOME_MINIMO) e não é letra nem espaço
            // (violaria NOME_INVALIDO). Em Verifica(), o check de formato roda ANTES
            // do check de tamanho mínimo, então a mensagem esperada é NOME_INVALIDO.
            const string primeiroInvalidoECurto = "1";
            const string segundo = "Silva";

            // Act
            var acao = () => Nome.Create(primeiroInvalidoECurto, segundo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_INVALIDO);
        }

        [Fact]
        public void Create_NomeComCaractereInvalidoEMaiorQueOMaximo_DeveLancarMensagemInvalidaNaoMaximo()
        {
            // Arrange
            // String com 101 caracteres (> 100, violaria NOME_MAXIMO) contendo um
            // dígito no final (violaria NOME_INVALIDO). Em Verifica(), o check de
            // formato roda ANTES do check de tamanho máximo.
            var primeiroInvalidoELongo = CriarNomeComTamanhoExato(100) + "1";
            const string segundo = "Silva";

            // Act
            var acao = () => Nome.Create(primeiroInvalidoELongo, segundo);

            // Assert
            acao.Should().Throw<UsuariosValidacao>()
                .WithMessage(MensagensUsuarios.NOME_INVALIDO);
        }

        // ---------------------------------------------------------------
        // Igualdade de Value Object
        // ---------------------------------------------------------------

        [Fact]
        public void Equals_DoisNomesComMesmoPrimeiroESegundo_DevemSerIguais()
        {
            // Arrange
            var nome1 = Nome.Create("Joao", "Silva");
            var nome2 = Nome.Create("Joao", "Silva");

            // Act
            var saoIguais = nome1.Equals(nome2);

            // Assert
            saoIguais.Should().BeTrue();
            (nome1 == nome2).Should().BeTrue();
        }

        [Fact]
        public void Equals_NomesComPrimeiroDiferente_NaoDevemSerIguais()
        {
            // Arrange
            var nome1 = Nome.Create("Joao", "Silva");
            var nome2 = Nome.Create("Pedro", "Silva");

            // Act
            var saoIguais = nome1.Equals(nome2);

            // Assert
            saoIguais.Should().BeFalse();
            (nome1 != nome2).Should().BeTrue();
        }

        [Fact]
        public void Equals_NomesComSegundoDiferente_NaoDevemSerIguais()
        {
            // Arrange
            var nome1 = Nome.Create("Joao", "Silva");
            var nome2 = Nome.Create("Joao", "Souza");

            // Act
            var saoIguais = nome1.Equals(nome2);

            // Assert
            saoIguais.Should().BeFalse();
        }

        [Fact]
        public void Equals_NomesComMesmoValorEmCaixaDiferente_NaoDevemSerIguais()
        {
            // Arrange
            // Documenta a ausência de normalização de caixa: o VO trata "Joao"/"joao"
            // como valores distintos.
            var nome1 = Nome.Create("Joao", "Silva");
            var nome2 = Nome.Create("joao", "silva");

            // Act
            var saoIguais = nome1.Equals(nome2);

            // Assert
            saoIguais.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_NomesIguais_DevemTerMesmoHashCode()
        {
            // Arrange
            var nome1 = Nome.Create("Joao", "Silva");
            var nome2 = Nome.Create("Joao", "Silva");

            // Act
            var hash1 = nome1.GetHashCode();
            var hash2 = nome2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void Create_MesmosNomesEmInstanciasDiferentes_DevemSerIguaisPorValorMasNaoPorReferencia()
        {
            // Arrange
            var nome1 = Nome.Create("Joao", "Silva");
            var nome2 = Nome.Create("Joao", "Silva");

            // Act
            var saoAMesmaReferencia = ReferenceEquals(nome1, nome2);
            var saoIguaisPorValor = nome1.Equals(nome2);

            // Assert
            saoAMesmaReferencia.Should().BeFalse();
            saoIguaisPorValor.Should().BeTrue();
        }
    }
}