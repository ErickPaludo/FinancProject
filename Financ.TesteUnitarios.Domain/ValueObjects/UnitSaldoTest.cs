using System;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using FluentAssertions;
using Xunit;

namespace Financ.TesteUnitarios.Domain.ValueObjects
{
    public class UnitSaldoTest
    {
        [Theory]
        [InlineData(100.50)]
        [InlineData(0.00)]
        [InlineData(-50.75)]
        public void Create_ValorInformado_DeveCriarInstanciaComValorCorreto(decimal valorInformado)
        {
            // Arrange & Act
            var saldo = Saldo.Create(valorInformado);

            // Assert
            saldo.Valor.Should().Be(valorInformado);
        }

        [Fact]
        public void Soma_SaldoValido_DeveRetornarNovoSaldoComSomaDosValores()
        {
            // Arrange
            var saldoOriginal = Saldo.Create(100.00m);
            var saldoParaSomar = Saldo.Create(50.00m);

            // Act
            var saldoResultado = saldoOriginal.Soma(saldoParaSomar);

            // Assert
            saldoResultado.Valor.Should().Be(150.00m);
            // Garantir que a imutabilidade foi preservada
            saldoOriginal.Valor.Should().Be(100.00m);
            saldoParaSomar.Valor.Should().Be(50.00m);
        }

        [Fact]
        public void Soma_SaldoNulo_DeveLancarExceptionDomain()
        {
            // Arrange
            var saldoOriginal = Saldo.Create(100.00m);
            Saldo saldoNulo = null!;

            // Act
            Action action = () => saldoOriginal.Soma(saldoNulo);

            // Assert
            action.Should().Throw<ExceptionDomain>()
                .WithMessage(MensagensMovimentacao.VALOR_NULO);
        }

        [Fact]
        public void Subtrai_SaldoValido_DeveRetornarNovoSaldoComSubtracaoDosValores()
        {
            // Arrange
            var saldoOriginal = Saldo.Create(100.00m);
            var saldoParaSubtrair = Saldo.Create(30.00m);

            // Act
            var saldoResultado = saldoOriginal.Subtrai(saldoParaSubtrair);

            // Assert
            saldoResultado.Valor.Should().Be(70.00m);
            // Garantir que a imutabilidade foi preservada
            saldoOriginal.Valor.Should().Be(100.00m);
            saldoParaSubtrair.Valor.Should().Be(30.00m);
        }

        [Fact]
        public void Subtrai_ResultandoEmValorNegativo_DeveCalcularCorretamente()
        {
            // Arrange
            var saldoOriginal = Saldo.Create(50.00m);
            var saldoParaSubtrair = Saldo.Create(100.00m);

            // Act
            var saldoResultado = saldoOriginal.Subtrai(saldoParaSubtrair);

            // Assert
            // Documenta que o Value Object permite e calcula corretamente saldos devedores (negativos)
            saldoResultado.Valor.Should().Be(-50.00m);
        }

        [Fact]
        public void Subtrai_SaldoNulo_DeveLancarExceptionDomain()
        {
            // Arrange
            var saldoOriginal = Saldo.Create(100.00m);
            Saldo saldoNulo = null!;

            // Act
            Action action = () => saldoOriginal.Subtrai(saldoNulo);

            // Assert
            action.Should().Throw<ExceptionDomain>()
                .WithMessage(MensagensMovimentacao.VALOR_NULO);
        }

        [Fact]
        public void Equals_InstanciasDiferentesComMesmoValor_DevemSerConsideradasIguais()
        {
            // Arrange
            var saldo1 = Saldo.Create(100.50m);
            var saldo2 = Saldo.Create(100.50m);

            // Act
            var saoIguais = saldo1.Equals(saldo2);

            // Assert
            saoIguais.Should().BeTrue();
            (saldo1 == saldo2).Should().BeTrue();
        }

        [Fact]
        public void Equals_InstanciasComValoresDiferentes_NaoDevemSerConsideradasIguais()
        {
            // Arrange
            var saldo1 = Saldo.Create(100.50m);
            var saldo2 = Saldo.Create(50.00m);

            // Act
            var saoIguais = saldo1.Equals(saldo2);

            // Assert
            saoIguais.Should().BeFalse();
            (saldo1 != saldo2).Should().BeTrue();
        }
    }
}