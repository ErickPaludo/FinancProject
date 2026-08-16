using System;
using Financ.Domain.Objetos_de_Valor.ContaBancaria;
using Financ.Domain.Validacoes.ContasBancarias;
using FluentAssertions;
using Xunit;

namespace Financ.TesteUnitarios.Domain.ValueObjects.ContaBancaria
{
    public class UnitLimiteAcessosTest
    {
        [Fact]
        public void Create_DeveCriarInstanciaComValoresPadrao()
        {
            // Act
            var limite = LimiteAcessos.Create();

            // Assert
            limite.LimiteMestres.Should().Be(2);
            limite.LimiteAdministradores.Should().Be(5);
            limite.LimiteVisualizadores.Should().Be(5);
            limite.MaxUsuario.Should().Be(12);
        }

        [Theory]
        [InlineData(11, true)]
        [InlineData(12, false)]
        [InlineData(13, false)]
        public void DisposicaoAcessos_DeveRetornarSeQuantidadePermitidaCorretamente(int quantidade, bool esperado)
        {
            // Arrange
            var limite = LimiteAcessos.Create(); // MaxUsuario é 12

            // Act
            var resultado = limite.DisposicaoAcessos(quantidade);

            // Assert
            resultado.Should().Be(esperado);
        }

        [Fact]
        public void Alterar_ValoresValidos_DeveRetornarNovaInstanciaComNovosValores()
        {
            // Arrange
            var limiteOriginal = LimiteAcessos.Create();

            // Act
            var novoLimite = limiteOriginal.Alterar(5, 10, 10);

            // Assert
            novoLimite.LimiteMestres.Should().Be(5);
            novoLimite.LimiteAdministradores.Should().Be(10);
            novoLimite.LimiteVisualizadores.Should().Be(10);
            novoLimite.MaxUsuario.Should().Be(25);

            // Garantir que a imutabilidade foi preservada
            limiteOriginal.LimiteMestres.Should().Be(2);
        }

        [Fact]
        public void Alterar_MestresMenorQueUm_DeveLancarExcecao()
        {
            // Arrange
            var limite = LimiteAcessos.Create();

            // Act
            Action action = () => limite.Alterar(0, 5, 5);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage("O número máximo de mestres deve ser maior que zero.");
        }

        [Fact]
        public void Alterar_MestresMaiorQueDez_DeveLancarExcecao()
        {
            // Arrange
            var limite = LimiteAcessos.Create();

            // Act
            Action action = () => limite.Alterar(11, 5, 5);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage("O número máximo de mestres é 10.");
        }

        [Fact]
        public void Alterar_AdministradoresNegativo_DeveLancarExcecao()
        {
            // Arrange
            var limite = LimiteAcessos.Create();

            // Act
            Action action = () => limite.Alterar(2, -1, 5);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage("O número máximo de administradores não pode ser negativo.");
        }

        [Fact]
        public void Alterar_AdministradoresMaiorQueQuinze_DeveLancarExcecao()
        {
            // Arrange
            var limite = LimiteAcessos.Create();

            // Act
            Action action = () => limite.Alterar(2, 16, 5);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage("O número máximo de administradores é 15.");
        }

        [Fact]
        public void Alterar_VisualizadoresNegativo_DeveLancarExcecao()
        {
            // Arrange
            var limite = LimiteAcessos.Create();

            // Act
            Action action = () => limite.Alterar(2, 5, -1);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage("O número máximo de visualizadores não pode ser negativo.");
        }

        [Fact]
        public void Alterar_VisualizadoresMaiorQueQuinze_DeveLancarExcecao()
        {
            // Arrange
            var limite = LimiteAcessos.Create();

            // Act
            Action action = () => limite.Alterar(2, 5, 16);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage("O número máximo de visualizadores é 15.");
        }
    }
}
