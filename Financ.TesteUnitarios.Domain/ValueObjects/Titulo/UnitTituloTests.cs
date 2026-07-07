using Financ.Domain.Objetos_de_Valor.Titulo;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Financ.TesteUnitarios.Domain.ValueObjects.Titulo
{
    public class UnitTituloTests
    {
        private static string GerarString(int tamanho) => new string('a', tamanho);

        #region TituloBase (Comportamento Comum)

        [Fact]
        public void Create_TituloNulo_DeveLancarExceptionDomain()
        {
            // Arrange
            string tituloNulo = null;

            // Act
            Action action = () => TituloCategoria.Create(tituloNulo);

            // Assert
            action.Should().Throw<ExceptionDomain>()
                .WithMessage(MensagensBase.TITULO_NULO);
        }

        [Theory]
        [InlineData("  mercado  ", "Mercado")]
        [InlineData("MERCADO LIVRE", "Mercado Livre")]
        [InlineData("conta corrente", "Conta Corrente")]
        public void Create_TituloComEspacosECaixaDiferente_DeveFormatarCorretamente(string tituloBruto, string tituloEsperado)
        {
            // Arrange & Act
            // O comportamento de ToTitleCase e Trim vem do TituloBase, mas testamos por meio de uma classe concreta
            var titulo = TituloConta.Create(tituloBruto);

            // Assert
            titulo.Texto.Should().Be(tituloEsperado);
        }

        [Fact]
        public void Equals_DoisTitulosComMesmoValor_DevemSerIguais()
        {
            // Arrange
            var titulo1 = TituloMovimentacao.Create("Compras do Mês");
            var titulo2 = TituloMovimentacao.Create("COMPRAS DO MÊS"); // Será formatado da mesma forma

            // Act
            var saoIguais = titulo1 == titulo2;

            // Assert
            saoIguais.Should().BeTrue();
        }

        #endregion

        #region TituloCategoria

        [Fact]
        public void Create_TituloCategoriaValido_DeveCriarComSucesso()
        {
            // Arrange
            var textoValido = "Alimentação";

            // Act
            var titulo = TituloCategoria.Create(textoValido);

            // Assert
            // CultureInfo.CurrentCulture.TextInfo.ToTitleCase pode transformar para "Alimentação"
            titulo.Texto.Should().BeEquivalentTo(textoValido);
        }

        [Fact]
        public void Create_TituloCategoriaMenorQueMinimo_DeveLancarExcecao()
        {
            // Arrange
            var textoCurto = "A"; // Mínimo é 2

            // Act
            Action action = () => TituloCategoria.Create(textoCurto);

            // Assert
            action.Should().Throw<ContasValidacao>()
                .WithMessage(MensagensBase.TITULO_TAMANHO_INVALIDO(2, 30));
        }

        [Fact]
        public void Create_TituloCategoriaMaiorQueMaximo_DeveLancarExcecao()
        {
            // Arrange
            var textoLongo = GerarString(31); // Máximo é 30

            // Act
            Action action = () => TituloCategoria.Create(textoLongo);

            // Assert
            action.Should().Throw<ContasValidacao>()
                .WithMessage(MensagensBase.TITULO_TAMANHO_INVALIDO(2, 30));
        }

        [Fact]
        public void Create_TituloCategoriaTamanhoMaximoExato_DeveCriarComSucesso()
        {
            // Arrange
            var textoLongo = GerarString(30);

            // Act
            var titulo = TituloCategoria.Create(textoLongo);

            // Assert
            titulo.Texto.Length.Should().Be(30);
        }

        #endregion

        #region TituloConta

        [Fact]
        public void Create_TituloContaValido_DeveCriarComSucesso()
        {
            // Arrange
            var textoValido = "Nubank Corrente";

            // Act
            var titulo = TituloConta.Create(textoValido);

            // Assert
            titulo.Texto.Should().BeEquivalentTo("Nubank Corrente");
        }

        [Fact]
        public void Create_TituloContaMenorQueMinimo_DeveLancarExcecao()
        {
            // Arrange
            var textoCurto = "N";

            // Act
            Action action = () => TituloConta.Create(textoCurto);

            // Assert
            action.Should().Throw<ContasValidacao>()
                .WithMessage(MensagensBase.TITULO_TAMANHO_INVALIDO(2, 45));
        }

        [Fact]
        public void Create_TituloContaMaiorQueMaximo_DeveLancarExcecao()
        {
            // Arrange
            var textoLongo = GerarString(46); // Máximo é 45

            // Act
            Action action = () => TituloConta.Create(textoLongo);

            // Assert
            action.Should().Throw<ContasValidacao>()
                .WithMessage(MensagensBase.TITULO_TAMANHO_INVALIDO(2, 45));
        }

        [Fact]
        public void Create_TituloContaTamanhoMaximoExato_DeveCriarComSucesso()
        {
            // Arrange
            var textoLongo = GerarString(45);

            // Act
            var titulo = TituloConta.Create(textoLongo);

            // Assert
            titulo.Texto.Length.Should().Be(45);
        }

        #endregion

        #region TituloMovimentacao

        [Fact]
        public void Create_TituloMovimentacaoValido_DeveCriarComSucesso()
        {
            // Arrange
            var textoValido = "Compra No Mercado Livre";

            // Act
            var titulo = TituloMovimentacao.Create(textoValido);

            // Assert
            titulo.Texto.Should().BeEquivalentTo("Compra No Mercado Livre");
        }

        [Fact]
        public void Create_TituloMovimentacaoMenorQueMinimo_DeveLancarExcecao()
        {
            // Arrange
            var textoCurto = "O";

            // Act
            Action action = () => TituloMovimentacao.Create(textoCurto);

            // Assert
            action.Should().Throw<ContasValidacao>()
                .WithMessage(MensagensBase.TITULO_TAMANHO_INVALIDO(2, 50));
        }

        [Fact]
        public void Create_TituloMovimentacaoMaiorQueMaximo_DeveLancarExcecao()
        {
            // Arrange
            var textoLongo = GerarString(51); // Máximo é 50

            // Act
            Action action = () => TituloMovimentacao.Create(textoLongo);

            // Assert
            action.Should().Throw<ContasValidacao>()
                .WithMessage(MensagensBase.TITULO_TAMANHO_INVALIDO(2, 50));
        }

        [Fact]
        public void Create_TituloMovimentacaoTamanhoMaximoExato_DeveCriarComSucesso()
        {
            // Arrange
            var textoLongo = GerarString(50);

            // Act
            var titulo = TituloMovimentacao.Create(textoLongo);

            // Assert
            titulo.Texto.Length.Should().Be(50);
        }

        #endregion
    }
}
