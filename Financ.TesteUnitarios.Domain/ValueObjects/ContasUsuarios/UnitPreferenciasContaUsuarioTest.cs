using Financ.Domain.Objetos_de_Valor.ContaUsuario;
using FluentAssertions;
using Xunit;

namespace Financ.TesteUnitarios.Domain.ValueObjects.ContaUsuario
{
    public class UnitPreferenciasContaUsuarioTest
    {
        [Fact]
        public void Create_Instanciacao_DeveRetornarValoresPadrao()
        {
            // Arrange & Act
            var preferencias = PreferenciasContaUsuario.Create();

            // Assert
            preferencias.ContaFavorita.Should().BeFalse();
            preferencias.AutoSoma.Should().BeTrue();
        }

        [Fact]
        public void Favoritar_EstadoInicialFalso_DeveAlterarParaVerdadeiro()
        {
            // Arrange
            var preferencias = PreferenciasContaUsuario.Create();

            // Act
            preferencias.Favoritar();

            // Assert
            preferencias.ContaFavorita.Should().BeTrue();
        }

        [Fact]
        public void Favoritar_ChamadaDupla_DeveAlternarEstadoCorretamente()
        {
            // Arrange
            var preferencias = PreferenciasContaUsuario.Create();

            // Act
            preferencias.Favoritar(); // Altera para true
            preferencias.Favoritar(); // Altera para false novamente

            // Assert
            preferencias.ContaFavorita.Should().BeFalse();
        }

        [Fact]
        public void PerimiteAutoSoma_EstadoInicialVerdadeiro_DeveAlterarParaFalso()
        {
            // Arrange
            var preferencias = PreferenciasContaUsuario.Create();

            // Act
            preferencias.PerimiteAutoSoma();

            // Assert
            preferencias.AutoSoma.Should().BeFalse();
        }

        [Fact]
        public void PerimiteAutoSoma_ChamadaDupla_DeveAlternarEstadoCorretamente()
        {
            // Arrange
            var preferencias = PreferenciasContaUsuario.Create();

            // Act
            preferencias.PerimiteAutoSoma(); // Altera para false
            preferencias.PerimiteAutoSoma(); // Altera para true novamente

            // Assert
            preferencias.AutoSoma.Should().BeTrue();
        }

        [Fact]
        public void Equals_InstanciasDiferentesComMesmosValores_DevemSerConsideradasIguais()
        {
            // Arrange
            var preferencias1 = PreferenciasContaUsuario.Create();
            var preferencias2 = PreferenciasContaUsuario.Create();

            // Act
            var saoIguais = preferencias1.Equals(preferencias2);

            // Assert
            saoIguais.Should().BeTrue();
            (preferencias1 == preferencias2).Should().BeTrue();
        }

        [Fact]
        public void Equals_InstanciasComEstadosDiferentes_NaoDevemSerConsideradasIguais()
        {
            // Arrange
            var preferencias1 = PreferenciasContaUsuario.Create();
            var preferencias2 = PreferenciasContaUsuario.Create();

            // Act
            preferencias2.Favoritar(); // Muda o estado de um deles
            var saoIguais = preferencias1.Equals(preferencias2);

            // Assert
            saoIguais.Should().BeFalse();
            (preferencias1 != preferencias2).Should().BeTrue();
        }
    }
}