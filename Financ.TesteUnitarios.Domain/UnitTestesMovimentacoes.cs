using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using FluentAssertions;
using System;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesMovimentacoes
    {
        //private Conta CriarContaAtiva() => new Conta(1, "Conta Teste", "#FFFFFF");

        //private ContaUsuario CriarUsuarioAtivo(Conta conta, TiposAcessos acesso = TiposAcessos.Administrador)
        //{
        //    var usuario = new ContaUsuario(1, conta, "user-123", acesso, StatusContasUsuario.Ativo);
        //    conta.AddUsuario(usuario);
        //    return usuario;
        //}

        //private Categoria CriarCategoria(Conta conta)
        //{
        //    // Simula a criação de uma categoria para a conta
        //    // Nota: Categoria exige um ContaUsuario Mestre no construtor real, 
        //    // mas para o teste de Movimentacao, precisamos apenas do objeto Categoria vinculado à conta.
        //    var mestre = new ContaUsuario(conta, "mestre-cat");
        //    return new Categoria(mestre, "Categoria Teste", "#FFFFFF");
        //}

        //#region Construtor

        //[Fact]
        //public void Movimentacao_DeveCriarComSucesso_QuandoDadosForemValidos()
        //{
        //    // Arrange
        //    var conta = CriarContaAtiva();
        //    var usuario = CriarUsuarioAtivo(conta);
        //    var categoria = CriarCategoria(conta);
        //    var valor = 150.50m;
        //    var titulo = "Supermercado";

        //    // Act
        //    var movimentacao = new Movimentacao(TipoMovimentacao.Saida, usuario, valor, titulo, "Compras do mês", DateTime.UtcNow, null, false);

        //    // Assert
        //    movimentacao.Tipo.Should().Be(TipoMovimentacao.Saida);
        //    movimentacao.Valor.Should().Be(valor);
        //    movimentacao.Titulo.Should().Be(titulo);
        //    movimentacao.Status.Should().Be(StatusMovimentacao.Pendente);
        //    movimentacao.Conta.Should().Be(conta);
        //}

        //[Fact]
        //public void Movimentacao_DeveLancarExcecao_QuandoValorForZeroOuNegativo()
        //{
        //    // Arrange
        //    var conta = CriarContaAtiva();
        //    var usuario = CriarUsuarioAtivo(conta);

        //    // Act
        //    Action action = () => new Movimentacao(TipoMovimentacao.Entrada, usuario, 0, "Teste", null, null, null, false);

        //    // Assert
        //    action.Should().Throw<MovimentacaoValidacao>()
        //        .WithMessage(MensagemMovimentacao.VALOR_DEVE_SER_MAIOR_QUE_ZERO);
        //}

        //[Theory]
        //[InlineData("")]
        //[InlineData("  ")]
        //[InlineData("Ab")] // Menor que 3
        //public void Movimentacao_DeveLancarExcecao_QuandoTituloForInvalido(string tituloInvalido)
        //{
        //    // Arrange
        //    var conta = CriarContaAtiva();
        //    var usuario = CriarUsuarioAtivo(conta);

        //    // Act
        //    Action action = () => new Movimentacao(TipoMovimentacao.Entrada, usuario, 100, tituloInvalido, null, null, null, false);

        //    // Assert
        //    action.Should().Throw<MovimentacaoValidacao>();
        //}

        //[Fact]
        //public void Movimentacao_DeveLancarExcecao_QuandoUsuarioForApenasVisualizador()
        //{
        //    // Arrange
        //    var conta = CriarContaAtiva();
        //    var visualizador = CriarUsuarioAtivo(conta, TiposAcessos.Visualizador);

        //    // Act
        //    Action action = () => new Movimentacao(TipoMovimentacao.Entrada, visualizador, 100, "Teste", null, null, null, false);

        //    // Assert
        //    action.Should().Throw<MovimentacaoValidacao>()
        //        .WithMessage(MensagemMovimentacao.USUARIO_SEM_PERMISSAO);
        //}

        //#endregion

        //#region Execucao e Estorno

        //[Fact]
        //public void ExecutarMovimentacao_DeveAlterarStatusParaConcluido()
        //{
        //    // Arrange
        //    var conta = CriarContaAtiva();
        //    var usuario = CriarUsuarioAtivo(conta);
        //    var movimentacao = new Movimentacao(TipoMovimentacao.Saida, usuario, 50, "Lanche", null, DateTime.UtcNow, null, false);

        //    // Act
        //    movimentacao.ExecutarMovimentacao(usuario, DateTime.UtcNow);

        //    // Assert
        //    movimentacao.Status.Should().Be(StatusMovimentacao.Concluido);
        //    movimentacao.DthrConclusao.Should().NotBeNull();
        //}

        //[Fact]
        //public void ExecutarMovimentacao_DeveLancarExcecao_SeJaEstiverConcluida()
        //{
        //    // Arrange
        //    var conta = CriarContaAtiva();
        //    var usuario = CriarUsuarioAtivo(conta);
        //    var movimentacao = new Movimentacao(TipoMovimentacao.Saida, usuario, 50, "Lanche", null, DateTime.UtcNow, DateTime.UtcNow, true);

        //    // Act
        //    Action action = () => movimentacao.ExecutarMovimentacao(usuario, DateTime.UtcNow);

        //    // Assert
        //    action.Should().Throw<MovimentacaoValidacao>()
        //        .WithMessage(MensagemMovimentacao.MOVIMENTACAO_COM_STATUS_IGUAL_NA_EXECUCAO);
        //}

        //[Fact]
        //public void ExtornaMovimentacao_DeveVoltarParaPendente()
        //{
        //    // Arrange
        //    var conta = CriarContaAtiva();
        //    var usuario = CriarUsuarioAtivo(conta);
        //    var movimentacao = new Movimentacao(TipoMovimentacao.Saida, usuario, 50, "Lanche", null, DateTime.UtcNow, DateTime.UtcNow, true);

        //    // Act
        //    movimentacao.ExtornaMovimentacao(usuario);

        //    // Assert
        //    movimentacao.Status.Should().Be(StatusMovimentacao.Pendente);
        //    movimentacao.Extorno.Should().BeTrue();
        //    movimentacao.DthrConclusao.Should().BeNull();
        //}

        //#endregion

        //#region Alteracao

        //[Fact]
        //public void AlterarMovimentacao_DeveLancarExcecao_AoTentarMudarValorDeConcluida()
        //{
        //    // Arrange
        //    var conta = CriarContaAtiva();
        //    var usuario = CriarUsuarioAtivo(conta);
        //    var movimentacao = new Movimentacao(TipoMovimentacao.Saida, usuario, 50, "Lanche", null, DateTime.UtcNow, DateTime.UtcNow, true);

        //    // Act
        //    Action action = () => movimentacao.AlterarMovimentacao(usuario, 100m, null, null, null, null, null);

        //    // Assert
        //    action.Should().Throw<MovimentacaoValidacao>()
        //        .WithMessage(MensagemMovimentacao.NAO_PODE_ALTERAR_VALOR_DE_MOVIMENTACAO_CONCLUIDA);
        //}

        //#endregion
    }
}
