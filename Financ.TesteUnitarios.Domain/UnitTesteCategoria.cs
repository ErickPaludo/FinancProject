using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Validacoes.Cor;
using Financ.Domain.Validacoes.Cor.Mensagens;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using FluentAssertions;
using System;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTesteCategoria
    {
        private Conta CriarContaValida() => new Conta(1, "Conta Teste", "#FFFFFF");

        private ContaUsuario CriarUsuarioMestreAtivo(Conta conta)
        {
            // Usando o construtor: public ContaUsuario(Conta conta, string idUasuario)
            // Este construtor define automaticamente Acesso = Mestre e Status = Ativo
            return new ContaUsuario(conta, "user-123");
        }

        [Fact]
        public void Categoria_DeveCriarComSucesso_QuandoDadosForemValidos()
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuarioMestreAtivo(conta);
            var nome = "Alimentação";
            var corHex = "#FF0000";

            // Act
            var categoria = new Categoria(usuarioMestre, nome, corHex);

            // Assert
            categoria.Nome.Should().Be(nome);
            categoria.Cor.Valor.Should().Be(corHex);
            categoria.IdConta.Should().Be(conta.Id);
            categoria.Conta.Should().Be(conta);
        }

        [Fact]
        public void Categoria_DeveCriarComCorPadrao_QuandoCorForNula()
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuarioMestreAtivo(conta);
            var nome = "Transporte";
            string corNula = null;

            // Act
            var categoria = new Categoria(usuarioMestre, nome, corNula);

            // Assert
            categoria.Nome.Should().Be(nome);
            categoria.Cor.Valor.Should().Be("#1d293db3"); // Valor padrão definido em Cor.cs
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Categoria_DeveLancarExcecao_QuandoNomeForVazio(string nomeInvalido)
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuarioMestreAtivo(conta);
            var corHex = "#0000FF";

            // Act
            Action action = () => new Categoria(usuarioMestre, nomeInvalido, corHex);

            // Assert
            action.Should().Throw<CategoriaValidacao>()
                .WithMessage(MensagemCategoria.NOME_OBRIGATORIO);
        }

        [Theory]
        [InlineData("A")] // Menor que 2 (Regra: valor.Length < 2 || valor.Length > 50)
        [InlineData("Este nome de categoria é propositalmente muito longo para testar o limite de cinquenta caracteres")] // Maior que 50
        public void Categoria_DeveLancarExcecao_QuandoNomeTiverTamanhoInvalido(string nomeInvalido)
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuarioMestreAtivo(conta);
            var corHex = "#0000FF";

            // Act
            Action action = () => new Categoria(usuarioMestre, nomeInvalido, corHex);

            // Assert
            action.Should().Throw<CategoriaValidacao>()
                .WithMessage(MensagemCategoria.CARACTERES_INVALIDOS);
        }

        [Fact]
        public void Categoria_DeveLancarExcecao_QuandoUsuarioNaoForMestre()
        {
            // Arrange
            var conta = CriarContaValida();
            // Criando um usuário que não é mestre (usando construtor completo)
            var usuarioComum = new ContaUsuario(1, conta, "user-123", TiposAcessos.Administrador, TipoStatusContasUsuario.Ativo);

            // Act
            Action action = () => new Categoria(usuarioComum, "Teste", "#FFFFFF");

            // Assert
            action.Should().Throw<CategoriaValidacao>()
                .WithMessage("Usuário deve possuir acesso mestre para essa implementação.");
        }

        [Fact]
        public void Categoria_DeveLancarExcecao_QuandoUsuarioEstiverInativo()
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioInativo = new ContaUsuario(1, conta, "user-123", TiposAcessos.Mestre, TipoStatusContasUsuario.Inativo);

            // Act
            Action action = () => new Categoria(usuarioInativo, "Teste", "#FFFFFF");

            // Assert
            action.Should().Throw<CategoriaValidacao>()
                .WithMessage("Usuário não está ativo!");
        }

        [Fact]
        public void Alterar_DeveAtualizarDados_QuandoDadosForemValidos()
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuarioMestreAtivo(conta);
            var categoria = new Categoria(usuarioMestre, "Original", "#000000");
            var novoNome = "Alterado";
            var novaCor = "#FFFFFF";

            // Act
            categoria.Alterar(usuarioMestre, novoNome, novaCor);

            // Assert
            categoria.Nome.Should().Be(novoNome);
            categoria.Cor.Valor.Should().Be(novaCor);
        }

        [Fact]
        public void Alterar_NaoDeveAlterarNome_QuandoNomeForNulo()
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuarioMestreAtivo(conta);
            var nomeOriginal = "Original";
            var categoria = new Categoria(usuarioMestre, nomeOriginal, "#000000");

            // Act
            categoria.Alterar(usuarioMestre, null, "#FFFFFF");

            // Assert
            categoria.Nome.Should().Be(nomeOriginal);
            categoria.Cor.Valor.Should().Be("#FFFFFF");
        }

        [Fact]
        public void Remover_DeveExecutarSemExcecao_QuandoUsuarioForMestreAtivo()
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuarioMestreAtivo(conta);
            var categoria = new Categoria(usuarioMestre, "Teste", "#FFFFFF");

            // Act
            Action action = () => categoria.Remover(usuarioMestre);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Categoria_DeveLancarExcecao_QuandoCorForInvalida()
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuarioMestreAtivo(conta);
            var corInvalida = "invalid-hex";

            // Act
            Action action = () => new Categoria(usuarioMestre, "Teste", corInvalida);

            // Assert
            action.Should().Throw<CorValidacao>()
                .WithMessage(MensagemCor.COR_INVALIDA);
        }
    }
}
