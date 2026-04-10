using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using FluentAssertions;
using System.Drawing;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class ContaTests
    {
        private ContaUsuario CriarContaUsuario(Conta conta, string idUsuario, TiposAcessos acesso, TipoStatusContasUsuario status = TipoStatusContasUsuario.Ativo)
                => new ContaUsuario(
                    1,
                    conta,
                    idUsuario,
                    acesso,
                    status);

        private string NovoIdUsuario() => Guid.NewGuid().ToString();

        private Movimentacao CriaMovimentacao(TipoMovimentacao tipo, ContaUsuario contaUsuario, Categoria? categoria, decimal valor, string titulo, string observacao, DateTime? dthrMovimentacao, int id = 1) =>  new Movimentacao(id, tipo, contaUsuario, categoria, valor, titulo, observacao, dthrMovimentacao);
    

        #region Construtor

        // Testes de Sucesso
        [Fact]
        public void Deve_Criar_Conta_Valida()
        {
            var conta = new Conta("Conta Teste", "#FFFFFF");

            conta.Titulo.Should().Be("Conta Teste");
            conta.Status.Should().Be(TiposStatusContas.Ativo);
            conta.TipoConta.Should().Be(TipoConta.Corrente);
        }

        // Testes de Erro
        [Fact]
        public void Deve_Lancar_Excecao_Quando_Id_Menor_Igual_Zero()
        {
            Action act = () => new Conta(0, "Conta", "#FFFFFF");

            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Deve_Lancar_Excecao_Quando_Titulo_Invalido(string titulo)
        {
            Action act = () => new Conta(titulo, "#FFFFFF");

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Titulo_Menor_Que_3()
        {
            Action act = () => new Conta("ab", "#FFFFFF");

            act.Should().Throw<Exception>();
        }

        #endregion

        #region AtualizaConta

        // Testes de Sucesso
        [Fact]
        public void Deve_Atualizar_Titulo_Quando_Usuario_Tiver_Permissao()
        {
            var conta = new Conta("Conta Antiga", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            conta.AtualizaConta(usuario, "Conta Nova", null);

            conta.Titulo.Should().Be("Conta Nova");
        }

        [Fact]
        public void Deve_Atualizar_Status_Quando_Informado()
        {
            var conta = new Conta("Conta Teste", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);


            conta.AtualizaConta(usuario, null, TiposStatusContas.Inativo);

            conta.Status.Should().Be(TiposStatusContas.Inativo);
        }

        [Fact]
        public void Nao_Deve_Alterar_Nada_Quando_Titulo_E_Status_Nulos()
        {
            var conta = new Conta("Conta Teste", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            conta.AtualizaConta(usuario, null, null);

            conta.Titulo.Should().Be("Conta Teste");
            conta.Status.Should().Be(TiposStatusContas.Ativo);
        }

        [Fact]
        public void Deve_Atualizar_Titulo_E_Status_Juntos()
        {
            var conta = new Conta("Conta Antiga", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            conta.AtualizaConta(usuario, "Conta Nova", TiposStatusContas.Inativo);

            conta.Titulo.Should().Be("Conta Nova");
            conta.Status.Should().Be(TiposStatusContas.Inativo);
        }

        // Testes de Erro
        [Fact]
        public void Deve_Lancar_Excecao_Quando_Usuario_Nulo()
        {
            var conta = new Conta("Conta Teste", "#FFFFFF");

            Action act = () => conta.AtualizaConta(null!, "Novo Titulo", null);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Usuario_For_Visualizador()
        {
            var conta = new Conta("Conta Teste", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);


            Action act = () => conta.AtualizaConta(usuario, "Novo Titulo", null);

            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(TipoStatusContasUsuario.Inativo)]
        [InlineData(TipoStatusContasUsuario.Bloqueado)]
        public void Deve_Lancar_Excecao_Quando_Usuario_Nao_For_Ativo(TipoStatusContasUsuario status)
        {
            var conta = new Conta("Conta Teste", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre, status);


            Action act = () => conta.AtualizaConta(usuario, "Novo Titulo", null);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Usuario_Pertencer_A_Otra_Conta()
        {
            var conta1 = new Conta("Conta 1", "#FFFFFF");
            var conta2 = new Conta("Conta 2", "#FFFFFF");

            var usuario = CriarContaUsuario(conta2, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () => conta1.AtualizaConta(usuario, "Novo Titulo", null);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Titulo_Invalido_Na_Atualizacao()
        {
            var conta = new Conta("Conta Teste", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () => conta.AtualizaConta(usuario, "", null);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Usuario_For_Administrador()
        {
            var conta = new Conta("Conta Teste", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador);

            Action act = () => conta.AtualizaConta(usuario, "Novo Titulo", null);

            act.Should().Throw<Exception>();
        }

        #endregion
        #region Movimentaçoes
        [Fact]
        public void Conclui_Movimentaca_De_Entrada_com_Sucesso()
        {
            var conta = new Conta(1, "Conta Teste", "#FFFFFF");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var movimentacao = CriaMovimentacao(TipoMovimentacao.Entrada, usuario, null, 100, "Salário", "Recebimento do salário", DateTime.UtcNow);
            conta.ProcessaMovimentacao(movimentacao);
            conta.Should();
        }
        #endregion
    }
}
