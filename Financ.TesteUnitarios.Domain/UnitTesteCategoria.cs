using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Cor;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Cor.Mensagens;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using FluentAssertions;
using System;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesCategoria
    {
        private Conta CriarContaValida() => new Conta(1, "Conta Teste", "#FFFFFF");

        [Fact]
        public void Categoria_DeveCriarComSucesso_QuandoDadosForemValidos()
        {
            // Arrange
            var conta = CriarContaValida();
            var nome = "Alimentação";
            var corHex = "#FF0000";

            // Act
            var categoria = new Categoria(nome, corHex);

            // Assert
            categoria.Nome.Should().Be(nome);
            categoria.Cor.Valor.Should().Be(corHex);
        }

        [Fact]
        public void Categoria_DeveCriarSemConta_QuandoDadosForemValidos()
        {
            // Arrange
            var nome = "Transporte";
            var corHex = "#00FF00";

            // Act
            var categoria = new Categoria(nome, corHex);

            // Assert
            categoria.Nome.Should().Be(nome);
            categoria.Cor.Valor.Should().Be(corHex);
            categoria.Conta.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Categoria_DeveLancarExcecao_QuandoNomeForVazio(string nomeInvalido)
        {
            // Arrange
            var corHex = "#0000FF";

            // Act
            Action action = () => new Categoria(nomeInvalido, corHex);

            // Assert
            action.Should().Throw<MovimentacaoValidacao>()
                .WithMessage(MensagemCategoria.NOME_OBRIGATORIO);
        }

        [Theory]
        [InlineData("Ab")] // Menor que 3
        [InlineData("Este nome de categoria é propositalmente muito longo para testar o limite de cinquenta caracteres")] // Maior que 50
        public void Categoria_DeveLancarExcecao_QuandoNomeTiverTamanhoInvalido(string nomeInvalido)
        {
            // Arrange
            var corHex = "#0000FF";

            // Act
            Action action = () => new Categoria(nomeInvalido, corHex);

            // Assert
            action.Should().Throw<MovimentacaoValidacao>()
                .WithMessage(MensagemCategoria.CARACTERES_INVALIDOS);
        }

        [Fact]
        public void Cor_DeveCriarComSucesso_QuandoHexadecimalForValido()
        {
            // Arrange
            var hexValido = "#ABCDEF";

            // Act
            var cor = new Cor(hexValido);

            // Assert
            cor.Valor.Should().Be(hexValido);
        }

        [Theory]
        [InlineData("ABCDEF")] // Sem #
        [InlineData("#ABCDE")]  // 5 caracteres
        [InlineData("#ABCDEFG")] // 7 caracteres
        [InlineData("#GHIJKL")] // Caracteres não hex
        [InlineData("")]        // Vazio
        [InlineData(null)]      // Null
        public void Cor_DeveLancarExcecao_QuandoHexadecimalForInvalido(string hexInvalido)
        {
            // Act
            Action action = () => new Cor(hexInvalido);

            // Assert
            action.Should().Throw<CorValidacao>()
                .WithMessage(MensagemCor.COR_INVALIDA);
        }
    }
}
