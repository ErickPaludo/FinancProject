using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class ContaTests
    {
        #region Construtor
        private ContasUsuarios CriarContaUsuario(Conta conta, string idUsuario, TiposAcessos acesso)
                => new ContasUsuarios(
                    1,
                    conta,
                    idUsuario,
                    acesso);

        private string NovoIdUsuario() => Guid.NewGuid().ToString();

        [Fact]
        public void Deve_Criar_Conta_Valida()
        {
            var conta = new Conta("Conta Teste");

            conta.Titulo.Should().Be("Conta Teste");
            conta.Status.Should().Be(TiposStatusContas.Ativo);
            conta.TipoConta.Should().Be(TiposContas.Corrente);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Id_Menor_Igual_Zero()
        {
            Action act = () => new Conta(0, "Conta");

            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Deve_Lancar_Excecao_Quando_Titulo_Invalido(string titulo)
        {
            Action act = () => new Conta(titulo);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Titulo_Menor_Que_3()
        {
            Action act = () => new Conta("ab");

            act.Should().Throw<Exception>();
        }

        #endregion

        #region AtualizaConta

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Usuario_Nulo()
        {
            var conta = new Conta("Conta Teste");

            Action act = () => conta.AtualizaConta(null!, "Novo Titulo", null);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Usuario_For_Visualizador()
        {
            var conta = new Conta("Conta Teste");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);


            Action act = () => conta.AtualizaConta(usuario, "Novo Titulo", null);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Deve_Atualizar_Titulo_Quando_Usuario_Tiver_Permissao()
        {
            var conta = new Conta("Conta Antiga");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            conta.AtualizaConta(usuario, "Conta Nova", null);

            conta.Titulo.Should().Be("Conta Nova");
        }

        [Fact]
        public void Deve_Atualizar_Status_Quando_Informado()
        {
            var conta = new Conta("Conta Teste");

            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);


            conta.AtualizaConta(usuario, null, TiposStatusContas.Inativo);

            conta.Status.Should().Be(TiposStatusContas.Inativo);
        }

        #endregion
    }
}