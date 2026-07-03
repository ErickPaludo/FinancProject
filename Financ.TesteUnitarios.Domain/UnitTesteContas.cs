using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTesteContas
    {
        //private Conta CriarContaValida(int id = 1) => new Conta(id, "Conta Teste", "#FFFFFF");

        //private ContaUsuario CriarUsuarioMestreAtivo(Conta conta, string idUsuario = "user-123")
        //{
        //    var usuario = new ContaUsuario(conta, idUsuario);
        //    conta.AddUsuario(usuario);
        //    return usuario;
        //}

        //private Movimentacao CriarMovimentacao(TipoMovimentacao tipo, decimal valor, ContaUsuario usuario, bool concluido = true)
        //{
        //    return new Movimentacao(tipo, usuario, valor, "Teste", null, DateTime.UtcNow, concluido ? DateTime.UtcNow : null, concluido);
        //}

        //#region Construtor

        //[Fact]
        //public void Conta_DeveCriarComSucesso_QuandoDadosForemValidos()
        //{
        //    // Arrange
        //    var titulo = "Minha Conta";
        //    var cor = "#ABCDEF";

        //    // Act
        //    var conta = new Conta(titulo, cor);

        //    // Assert
        //    conta.Titulo.Should().Be(titulo);
        //    conta.Cor.Valor.Should().Be(cor);
        //    conta.Status.Should().Be(StatusContas.Ativo);
        //    conta.TipoConta.Should().Be(TipoConta.Corrente);
        //    conta.Saldo.Should().Be(0);
        //}

        //[Fact]
        //public void Conta_DeveLancarExcecao_QuandoIdForInvalido()
        //{
        //    // Act
        //    Action action = () => new Conta(0, "Teste", "#FFFFFF");

        //    // Assert
        //    action.Should().Throw<ContasValidacao>()
        //        .WithMessage(MensagensBase.ID_IGUAL_MENOR_ZERO);
        //}

        //[Theory]
        //[InlineData("")]
        //[InlineData("  ")]
        //[InlineData(null)]
        //public void Conta_DeveLancarExcecao_QuandoTituloForVazio(string tituloInvalido)
        //{
        //    // Act
        //    Action action = () => new Conta(tituloInvalido, "#FFFFFF");

        //    // Assert
        //    action.Should().Throw<ContasValidacao>()
        //        .WithMessage(MensagensContas.TITULO_OBRIGATORIO);
        //}

        //[Theory]
        //[InlineData("Ab")] // Menor que 3
        //[InlineData("Este título de conta é propositalmente muito longo para testar o limite de cem caracteres definidos na regra de negócio do sistema financeiro")] // Maior que 100
        //public void Conta_DeveLancarExcecao_QuandoTituloTiverTamanhoInvalido(string tituloInvalido)
        //{
        //    // Act
        //    Action action = () => new Conta(tituloInvalido, "#FFFFFF");

        //    // Assert
        //    action.Should().Throw<ContasValidacao>()
        //        .WithMessage(MensagensContas.TITULO_TAMANHO_INVALIDO);
        //}

        //#endregion

        //#region Atualizacao

        //[Fact]
        //public void AtualizaConta_DeveAlterarDados_QuandoUsuarioForMestreAtivo()
        //{
        //    // Arrange
        //    var conta = CriarContaValida();
        //    var usuarioMestre = CriarUsuarioMestreAtivo(conta);
        //    var novoTitulo = "Novo Titulo";
        //    var novoStatus = StatusContas.Inativo;

        //    // Act
        //    conta.AtualizaConta(usuarioMestre, novoTitulo, novoStatus, "#000000");

        //    // Assert
        //    conta.Titulo.Should().Be(novoTitulo);
        //    conta.Status.Should().Be(novoStatus);
        //    conta.Cor.Valor.Should().Be("#000000");
        //}

        //[Fact]
        //public void AtualizaConta_DeveLancarExcecao_QuandoUsuarioNaoPertencerAConta()
        //{
        //    // Arrange
        //    var conta1 = CriarContaValida(1);
        //    var conta2 = CriarContaValida(2);
        //    var usuarioConta2 = CriarUsuarioMestreAtivo(conta2);

        //    // Act
        //    Action action = () => conta1.AtualizaConta(usuarioConta2, "Novo", null);

        //    // Assert
        //    action.Should().Throw<ContasValidacao>()
        //        .WithMessage(MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        //}

        //[Fact]
        //public void AtualizaConta_DeveLancarExcecao_QuandoUsuarioNaoForMestre()
        //{
        //    // Arrange
        //    var conta = CriarContaValida();
        //    var usuarioComum = new ContaUsuario(1, conta, "user-123", TiposAcessos.Administrador, StatusContasUsuario.Ativo);
        //    conta.AddUsuario(usuarioComum);

        //    // Act
        //    Action action = () => conta.AtualizaConta(usuarioComum, "Novo", null);

        //    // Assert
        //    action.Should().Throw<ContasValidacao>()
        //        .WithMessage(MensagensContas.ATUALIZA_CONTA_USUARIO_SEM_PERMISSAO);
        //}

        //#endregion

        //#region Saldo e Movimentacoes

        //[Fact]
        //public void ProcessaMovimentacao_DeveAumentarSaldo_EmEntradaConcluida()
        //{
        //    // Arrange
        //    var conta = CriarContaValida();
        //    var usuario = CriarUsuarioMestreAtivo(conta);
        //    var movimentacao = CriarMovimentacao(TipoMovimentacao.Entrada, 100, usuario);

        //    // Act
        //    conta.ProcessaMovimentacao(movimentacao);

        //    // Assert
        //    conta.Saldo.Should().Be(100);
        //}

        //[Fact]
        //public void ProcessaMovimentacao_DeveDiminuirSaldo_EmSaidaConcluida()
        //{
        //    // Arrange
        //    var conta = CriarContaValida();
        //    var usuario = CriarUsuarioMestreAtivo(conta);
        //    var movimentacaoInicar = CriarMovimentacao(TipoMovimentacao.Entrada, 500, usuario);
        //    conta.ProcessaMovimentacao(movimentacaoInicar);

        //    var movimentacao = CriarMovimentacao(TipoMovimentacao.Saida, 100, usuario);

        //    // Act
        //    conta.ProcessaMovimentacao(movimentacao);

        //    // Assert
        //    conta.Saldo.Should().Be(400);
        //}

        //[Fact]
        //public void ProcessaMovimentacao_DeveLancarExcecao_QuandoSaldoForInsuficiente()
        //{
        //    // Arrange
        //    var conta = CriarContaValida();
        //    var usuario = CriarUsuarioMestreAtivo(conta);
        //    var movimentacaoInicial = CriarMovimentacao(TipoMovimentacao.Entrada, 50, usuario);
        //    conta.ProcessaMovimentacao(movimentacaoInicial);

        //    var movimentacao = CriarMovimentacao(TipoMovimentacao.Saida, 100, usuario);

        //    // Act
        //    Action action = () => conta.ProcessaMovimentacao(movimentacao);

        //    // Assert
        //    action.Should().Throw<ContasValidacao>()
        //        .WithMessage(MensagensContas.SALDO_INSUFICIENTE);
        //}

        //[Fact]
        //public void ProcessaExtornoMovimentacao_DeveReverterSaldo_Corretamente()
        //{
        //    // Arrange
        //    var conta = CriarContaValida();
        //    var usuario = CriarUsuarioMestreAtivo(conta);

        //    // Simula uma entrada que será estornada
        //    var movimentacaoInicial = CriarMovimentacao(TipoMovimentacao.Entrada, 800, usuario, concluido: false);
        //    movimentacaoInicial.ExecutarMovimentacao(usuario);
        //    conta.ProcessaMovimentacao(movimentacaoInicial);

        //    var movimentacao = CriarMovimentacao(TipoMovimentacao.Entrada, 200, usuario, concluido: false);
        //    movimentacao.ExecutarMovimentacao(usuario);
        //    conta.ProcessaMovimentacao(movimentacao);

        //    movimentacao.ExtornaMovimentacao(usuario); // Define Extorno = true e Status = Pendente

        //    // Act
        //    conta.ProcessaExtornoMovimentacao(movimentacao);

        //    // Assert
        //    conta.Saldo.Should().Be(800); // 1000 - 200
        //}

        //#endregion

        //#region Gestao de Usuarios

        //[Fact]
        //public void SairDaConta_DeveRemoverUsuario_EDesativarContaSeForUltimo()
        //{
        //    // Arrange
        //    var conta = CriarContaValida();
        //    var usuario = CriarUsuarioMestreAtivo(conta);

        //    // Act
        //    conta.SairDaConta(usuario);

        //    // Assert
        //    conta.ContaUsuarios.Should().BeEmpty();
        //    conta.Status.Should().Be(StatusContas.Inativo);
        //    usuario.Status.Should().Be(StatusContasUsuario.Removido);
        //}

        //[Fact]
        //public void UsuarioPertenceConta_DeveRetornarVerdadeiro_ParaUsuarioAtivo()
        //{
        //    // Arrange
        //    var conta = CriarContaValida();
        //    var idUsuario = "user-123";
        //    var usuario = CriarUsuarioMestreAtivo(conta, idUsuario);

        //    // Act
        //    var pertence = conta.UsuarioPertenceConta(idUsuario);

        //    // Assert
        //    pertence.Should().BeTrue();
        //}

        //#endregion
    }
}
