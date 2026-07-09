using System;
using FluentAssertions;
using Xunit;
using Financ.Domain.Objetos_de_Valor.Observação;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Base.Mensagens;

namespace Financ.TesteUnitarios.Domain.ValueObjects.Observação
{
    public class UnitObservacaoTest
    {
        // ---------------------------------------------------------------
        // Helpers de massa de dados
        // ---------------------------------------------------------------
        private static string CriarTextoComTamanhoExato(int tamanho)
        {
            return new string('a', tamanho);
        }

        // ---------------------------------------------------------------
        // Create - Cenários Válidos
        // ---------------------------------------------------------------

        [Fact]
        public void Create_TextoValido_DeveCriarComSucesso()
        {
            // Arrange
            const string textoValido = "Compra de suprimentos para o mês.";

            // Act
            var observacao = ObservacaoMovimentacao.Create(textoValido);

            // Assert
            observacao.Texto.Should().Be(textoValido);
        }

        [Fact]
        public void Create_TextoComEspacosNasExtremidades_DeveRemoverEspacosAntesDeArmazenar()
        {
            // Arrange
            const string textoComEspacos = "   Pagamento da conta de luz   ";
            const string textoEsperado = "Pagamento da conta de luz";

            // Act
            var observacao = ObservacaoMovimentacao.Create(textoComEspacos);

            // Assert
            // Valida se o método Prepara() da classe base (ObservacaoBase) foi executado corretamente
            observacao.Texto.Should().Be(textoEsperado);
        }

        [Fact]
        public void Create_TextoVazio_DeveCriarObservacaoVaziaComSucesso()
        {
            // Arrange
            const string textoVazio = "";

            // Act
            var observacao = ObservacaoMovimentacao.Create(textoVazio);

            // Assert
            observacao.Texto.Should().BeEmpty();
        }

        [Fact]
        public void Create_TextoComTamanhoMaximoExato_DeveCriarComSucesso()
        {
            // Arrange
            // ObservacaoBase define TamanhoMaximo virtual como 400
            var textoLimiteMaximo = CriarTextoComTamanhoExato(400);

            // Act
            var observacao = ObservacaoMovimentacao.Create(textoLimiteMaximo);

            // Assert
            observacao.Texto.Should().HaveLength(400);
        }

        // ---------------------------------------------------------------
        // Create - Cenários Inválidos e Casos de Borda
        // ---------------------------------------------------------------

        [Fact]
        public void Create_TextoComTamanhoMaiorQueMaximo_DeveLancarMovimentacaoValidacao()
        {
            // Arrange
            var textoAcimaDoLimite = CriarTextoComTamanhoExato(401);

            // Act
            var acao = () => ObservacaoMovimentacao.Create(textoAcimaDoLimite);

            // Assert
            acao.Should().Throw<MovimentacaoValidacao>()
                .WithMessage(MensagensBase.OBSERVACAO_TAMANHO_INVALIDO(400));
        }

        [Fact]
        public void Create_TextoNulo_DeveLancarNullReferenceException()
        {
            // Arrange
            string textoNulo = null;

            // Act
            var acao = () => ObservacaoMovimentacao.Create(textoNulo);

            // Assert
            // Documenta a falha atual de modelagem onde `texto.Length` é chamado no método Valida
            // sem verificar se a string é nula antes, resultando em uma exceção de sistema.
            acao.Should().Throw<NullReferenceException>();
        }

        // ---------------------------------------------------------------
        // Igualdade de Value Object (Records)
        // ---------------------------------------------------------------

        [Fact]
        public void Equals_DuasObservacoesComMesmoTexto_DevemSerIguais()
        {
            // Arrange
            var obs1 = ObservacaoMovimentacao.Create("Mensalidade da academia");
            var obs2 = ObservacaoMovimentacao.Create("Mensalidade da academia");

            // Act
            var saoIguais = obs1.Equals(obs2);

            // Assert
            saoIguais.Should().BeTrue();
            (obs1 == obs2).Should().BeTrue();
        }

        [Fact]
        public void Equals_DuasObservacoesComTextosDiferentes_NaoDevemSerIguais()
        {
            // Arrange
            var obs1 = ObservacaoMovimentacao.Create("Compra de supermercado");
            var obs2 = ObservacaoMovimentacao.Create("Gasolina");

            // Act
            var saoIguais = obs1.Equals(obs2);

            // Assert
            saoIguais.Should().BeFalse();
            (obs1 != obs2).Should().BeTrue();
        }

        [Fact]
        public void Equals_TextosIguaisMasComCaixaDiferente_NaoDevemSerIguais()
        {
            // Arrange
            var obs1 = ObservacaoMovimentacao.Create("Material de escritório");
            var obs2 = ObservacaoMovimentacao.Create("MATERIAL DE ESCRITÓRIO");

            // Act
            var saoIguais = obs1.Equals(obs2);

            // Assert
            saoIguais.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_ObservacoesIguais_DevemTerMesmoHashCode()
        {
            // Arrange
            var obs1 = ObservacaoMovimentacao.Create("Pagamento pendente");
            var obs2 = ObservacaoMovimentacao.Create("Pagamento pendente");

            // Act
            var hash1 = obs1.GetHashCode();
            var hash2 = obs2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }
    }
}