using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using FluentAssertions;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class ContasUsuariosTests
    {
        private Conta CriarContaAtiva()
            => new Conta("Conta Teste");

        private Usuario CriarUsuario(string id)
    => new Usuario(id, "Nome", "Sobrenome", $"{id}@teste.com");

        private string NovoIdUsuario() => Guid.NewGuid().ToString();

        #region Construtores

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Construtor_ComIdInvalido_DeveLancarExcecao(int idInvalido)
        {
            var conta = CriarContaAtiva();

            Action act = () =>
                new ContasUsuarios(idInvalido, conta, "user-1", TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensBase.ID_IGUAL_MENOR_ZERO);
        }

        [Fact]
        public void Construtor_ComContaNula_DeveLancarExcecao()
        {
            Action act = () =>
                new ContasUsuarios(null!, "user-1", TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.CONTA_NAO_PODE_SER_NULA);
        }

        [Fact]
        public void Construtor_ComContaInativa_DeveLancarExcecao()
        {
            var conta = new Conta("Conta Teste");
            conta.AtualizaConta(
                new ContasUsuarios(conta, "admin", TiposAcessos.Mestre),
                null,
                TiposStatusContas.Inativo);

            Action act = () =>
                new ContasUsuarios(conta, "user-1", TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.CONTA_NAO_ESTA_ATIVA);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Construtor_ComIdUsuarioInvalido_DeveLancarExcecao(string idUsuario)
        {
            var conta = CriarContaAtiva();

            Action act = () =>
                new ContasUsuarios(conta, idUsuario!, TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.IDUSUARIO_INVALIDO);
        }

        [Fact]
        public void Construtor_Valido_DeveCriarComStatusAtivo()
        {
            var conta = CriarContaAtiva();

            var usuario = new ContasUsuarios(conta, "user-1", TiposAcessos.Administrador);

            usuario.Status.Should().Be(TiposStatusContas.Ativo);
            usuario.Acesso.Should().Be(TiposAcessos.Administrador);
        }

        [Fact]
        public void Construtor_ComUsuario_DeveCriarComoMestre()
        {
            var conta = CriarContaAtiva();
            var usuario = CriarUsuario(NovoIdUsuario()); // ajuste se necessário

            var contasUsuario = new ContasUsuarios(conta, usuario);

            contasUsuario.Acesso.Should().Be(TiposAcessos.Mestre);
            contasUsuario.Status.Should().Be(TiposStatusContas.Ativo);
        }

        #endregion

        #region AtualizaOutraContaUsuario

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteForOMesmoUsuario()
        {
            var conta = CriarContaAtiva();

            var usuario = new ContasUsuarios(conta, "user-1", TiposAcessos.Mestre);

            Action act = () =>
                usuario.AtualizaOutraContaUsuario(usuario, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteNaoForMestre()
        {
            var conta = CriarContaAtiva();

            var alvo = new ContasUsuarios(conta, "user-alvo", TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(conta, "user-admin", TiposAcessos.Administrador);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteEstiverInativo()
        {
            var conta = CriarContaAtiva();

            var alvo = new ContasUsuarios(conta, "user-alvo", TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(1,conta, "user-mestre", TiposAcessos.Mestre,TiposStatusContas.Inativo);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeAlvoForMestre()
        {
            var conta = CriarContaAtiva();

            var alvo = new ContasUsuarios(conta, "alvo", TiposAcessos.Mestre);
            var remetente = new ContasUsuarios(conta, "remetente", TiposAcessos.Mestre);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO);
        }

        [Fact]
        public void Atualiza_FluxoValido_DeveAtualizarAcessoEStatus()
        {
            var conta = CriarContaAtiva();

            var alvo = new ContasUsuarios(conta, "alvo", TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(conta, "remetente", TiposAcessos.Mestre);

            alvo.AtualizaOutraContaUsuario(
                remetente,
                TiposAcessos.Administrador,
                TiposStatusContas.Inativo);

            alvo.Acesso.Should().Be(TiposAcessos.Administrador);
            alvo.Status.Should().Be(TiposStatusContas.Inativo);
        }

        #endregion
    }
}