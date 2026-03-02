using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using FluentAssertions;
using System.Net.NetworkInformation;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace Financ.TesteUnitarios.Domain
{
    public class ContasUsuariosTests
    {
        private Conta CriarContaAtiva()
            => new Conta("Conta Teste");

        private Usuario CriarUsuario(string id)
    => new Usuario(id, "Nome", "Sobrenome", $"{id}@teste.com");

        private string NovoIdUsuario() => Guid.NewGuid().ToString();

        private Convites CriarConvite(TiposAcessos aceso, ContasUsuarios usuarioRemetente, string idUsuarioDestinatario)
            => new Convites(aceso, usuarioRemetente, idUsuarioDestinatario);

        private ContasUsuarios CriarContaUsuario(Conta conta, string idUsuario, TiposAcessos acesso = TiposAcessos.Mestre, TipoStatusContasUsuario status = TipoStatusContasUsuario.Ativo)
        {
            ContasUsuarios contaUsuario = new ContasUsuarios(
                1,
                conta,
                idUsuario,
                acesso,
                status);

            conta.AddUsuario(contaUsuario);
            return contaUsuario;
        }

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
                new ContasUsuarios(1, null, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.CONTA_NAO_PODE_SER_NULA);
        }

        [Fact]
        public void Construtor_ComContaInativa_DeveLancarExcecao()
        {
            var conta = new Conta("Conta Teste");
            var usuarioRemetente = CriarUsuario(NovoIdUsuario()).IdUsuario;
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario()).IdUsuario;
            var convite = new Convites(TiposAcessos.Mestre, CriarContaUsuario(conta, usuarioRemetente), usuarioDestinatario);

            conta.AtualizaConta(
                new ContasUsuarios(convite),
                null,
                TiposStatusContas.Inativo);

            Action act = () =>
                new ContasUsuarios(convite);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.CONTA_NAO_ESTA_ATIVA);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Construtor_ComIdUsuarioInvalido_DeveLancarExcecao(string idUsuario)
        {
            var conta = CriarContaAtiva();
            Convites convite = CriarConvite(TiposAcessos.Administrador, CriarContaUsuario(conta, NovoIdUsuario()), idUsuario);

            Action act = () =>
                new ContasUsuarios(1, conta, idUsuario, TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.IDUSUARIO_INVALIDO);
        }

        [Fact]
        public void Construtor_Valido_DeveCriarComStatusAtivo()
        {
            var conta = CriarContaAtiva();

            var usuario = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Administrador);

            usuario.Status.Should().Be(TipoStatusContasUsuario.Ativo);
            usuario.Acesso.Should().Be(TiposAcessos.Administrador);
        }

        [Fact]
        public void Construtor_ComUsuario_DeveCriarComoMestre()
        {
            var conta = CriarContaAtiva();
            var usuario = CriarUsuario(NovoIdUsuario()); // ajuste se necessário

            var contasUsuario = new ContasUsuarios(conta, usuario);

            contasUsuario.Acesso.Should().Be(TiposAcessos.Mestre);
            contasUsuario.Status.Should().Be(TipoStatusContasUsuario.Ativo);
        }

        #endregion

        #region AtualizaOutraContaUsuario

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteForOMesmoUsuario()
        {
            var conta = CriarContaAtiva();
            var usuario = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre);

            Action act = () =>
                usuario.AtualizaOutraContaUsuario(usuario, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteNaoForMestre()
        {
            var conta = CriarContaAtiva();

            var alvo = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(2, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Administrador);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteEstiverInativo()
        {
            var conta = CriarContaAtiva();

            var alvo = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(2, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre, TipoStatusContasUsuario.Inativo);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeAlvoForMestre()
        {
            var conta = CriarContaAtiva();

            var alvo = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre);
            var remetente = new ContasUsuarios(2, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO);
        }

        [Fact]
        public void Atualiza_FluxoValido_DeveAtualizarAcessoEStatus()
        {
            var conta = CriarContaAtiva();
            //1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Visualizador
            var alvo = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre);

            alvo.AtualizaOutraContaUsuario(
                remetente,
                TiposAcessos.Administrador,
                TipoStatusContasUsuario.Inativo);

            alvo.Acesso.Should().Be(TiposAcessos.Administrador);
            alvo.Status.Should().Be(TipoStatusContasUsuario.Inativo);
        }

        #endregion
    }
}