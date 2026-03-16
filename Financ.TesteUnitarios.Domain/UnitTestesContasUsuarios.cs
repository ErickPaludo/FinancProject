using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using FluentAssertions;
using Xunit;
using System;
using System.Linq;

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

        // Testes de Sucesso
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
            var usuario = CriarUsuario(NovoIdUsuario()).IdUsuario;

            var contasUsuario = new ContasUsuarios(conta, usuario);

            contasUsuario.Acesso.Should().Be(TiposAcessos.Mestre);
            contasUsuario.Status.Should().Be(TipoStatusContasUsuario.Ativo);
        }

        [Fact]
        public void Construtor_Convite_DeveCriarComAcessoAdministrador_QuandoLimiteMestresAtingido()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            var convite = new Convites(TiposAcessos.Mestre, mestre1, NovoIdUsuario());

            var mestre2 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            var contasUsuario = new ContasUsuarios(convite);

            contasUsuario.Acesso.Should().Be(TiposAcessos.Administrador);
            convite.Observacao.Should().Contain(MensagensContasUsuarios.MAX_MESTRES_CONVERTE_PARA_ADMIN);
        }

        // Testes de Erro
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
            var usuarioRemetente = CriarContaUsuario(conta, NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario()).IdUsuario;
            var convite = new Convites(TiposAcessos.Mestre, usuarioRemetente, usuarioDestinatario);

            // Simula a conta se tornando inativa ANTES da criação do ContasUsuarios pelo convite
            conta.AtualizaConta(usuarioRemetente, null, TiposStatusContas.Inativo);

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

            Action act = () =>
                new ContasUsuarios(1, conta, idUsuario, TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.IDUSUARIO_INVALIDO);
        }

        [Fact]
        public void Construtor_ComStatusInvalido_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            // Simula um valor de enum inválido (ex: 999)
            TipoStatusContasUsuario statusInvalido = (TipoStatusContasUsuario)999;

            Action act = () =>
                new ContasUsuarios(1, conta, NovoIdUsuario(), TiposAcessos.Administrador, statusInvalido);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensBase.STATUS_INVALIDO);
        }

        [Fact]
        public void Construtor_ComAcessoInvalido_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            // Simula um valor de enum inválido (ex: 999)
            TiposAcessos acessoInvalido = (TiposAcessos)999;

            Action act = () =>
                new ContasUsuarios(1, conta, NovoIdUsuario(), acessoInvalido);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_INVALIDO);
        }

        #endregion

        #region AtualizaOutraContaUsuario

        // Testes de Sucesso
        [Fact]
        public void Atualiza_FluxoValido_DeveAtualizarAcessoEStatus()
        {
            var conta = CriarContaAtiva();
            var alvo = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(2, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre);
            conta.AddUsuario(alvo);
            conta.AddUsuario(remetente);

            alvo.AtualizaOutraContaUsuario(
                remetente,
                TiposAcessos.Administrador,
                TipoStatusContasUsuario.Inativo);

            alvo.Acesso.Should().Be(TiposAcessos.Administrador);
            alvo.Status.Should().Be(TipoStatusContasUsuario.Inativo);
        }

        [Fact]
        public void Atualiza_ApenasAcesso_DeveAtualizarCorretamente()
        {
            var conta = CriarContaAtiva();
            var alvo = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(2, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre);
            conta.AddUsuario(alvo);
            conta.AddUsuario(remetente);

            alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            alvo.Acesso.Should().Be(TiposAcessos.Administrador);
            alvo.Status.Should().Be(TipoStatusContasUsuario.Ativo); // Status deve permanecer o mesmo
        }

        [Fact]
        public void Atualiza_ApenasStatus_DeveAtualizarCorretamente()
        {
            var conta = CriarContaAtiva();
            var alvo = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(2, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre);
            conta.AddUsuario(alvo);
            conta.AddUsuario(remetente);

            alvo.AtualizaOutraContaUsuario(remetente, null, TipoStatusContasUsuario.Bloqueado);

            alvo.Acesso.Should().Be(TiposAcessos.Visualizador); // Acesso deve permanecer o mesmo
            alvo.Status.Should().Be(TipoStatusContasUsuario.Bloqueado);
        }

        // Testes de Erro
        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteForOMesmoUsuario()
        {
            var conta = CriarContaAtiva();
            var usuario = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Mestre);

            Action act = () =>
                usuario.AtualizaOutraContaUsuario(usuario, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_TENTA_SE_ATUALIZAR);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteNaoForMestre()
        {
            var conta = CriarContaAtiva();

            var alvo = new ContasUsuarios(1, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Visualizador);
            var remetente = new ContasUsuarios(2, conta, CriarUsuario(NovoIdUsuario()).IdUsuario, TiposAcessos.Administrador);
            conta.AddUsuario(alvo);
            conta.AddUsuario(remetente);

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
            conta.AddUsuario(alvo);
            conta.AddUsuario(remetente);

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
            conta.AddUsuario(alvo);
            conta.AddUsuario(remetente);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO);
        }

        [Fact]
        public void Atualiza_Para_Mestre_Limite_Maximo()
        {
            var conta = CriarContaAtiva();

            var usuarioMestre = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var outroMestre = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Mestre, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensBase.LIMITE_USUARIOS_MESTRES);
        }

        [Fact]
        public void Atualiza_ComAcessoInvalido_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            TiposAcessos acessoInvalido = (TiposAcessos)999;

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, acessoInvalido, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_INVALIDO);
        }

        [Fact]
        public void Atualiza_ComStatusInvalido_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            TipoStatusContasUsuario statusInvalido = (TipoStatusContasUsuario)999;

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, null, statusInvalido);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensBase.STATUS_INVALIDO);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeUsuarioRemetenteNaoPertenceAContaDoAlvo()
        {
            var conta1 = CriarContaAtiva();
            var conta2 = CriarContaAtiva();

            var alvo = CriarContaUsuario(conta1, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetenteDeOutraConta = CriarContaUsuario(conta2, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetenteDeOutraConta, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        }

        #endregion

        #region SairDaConta

        // Testes de Sucesso
        [Fact]
        public void SairDaConta_DevePermitirSair_QuandoNaoForUnicoMestre()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var mestre2 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () => mestre1.SairDaConta();
            act.Should().NotThrow(); // Não deve lançar exceção
        }

        [Fact]
        public void SairDaConta_QuandoTiver_ConviteAtivo_GeraErro()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var mestre2 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var convidado = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador).IdUsuario;

            Convites convite = CriarConvite(TiposAcessos.Administrador, mestre2 ,convidado);

            mestre2.Conta.AddConvite(convite);

            Action act = () => mestre2.SairDaConta();

            act.Should().Throw<ContasUsuariosValidacao>()
                           .WithMessage(MensagensContasUsuarios.USUARIO_POSSUI_CONVITES_EM_ANDAMENTO);
        }


        [Fact]
        public void SairDaConta_DevePermitirSair_QuandoNaoForMestre()
        {
            var conta = CriarContaAtiva();
            var administrador = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador);

            Action act = () => administrador.SairDaConta();
            act.Should().NotThrow(); // Não deve lançar exceção
        }

        // Testes de Erro
        [Fact]
        public void SairDaConta_DeveFalhar_QuandoForUnicoMestreEExistiremOutrosUsuariosNaConta()
        {
            var conta = CriarContaAtiva();
            var unicoMestre = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var outroUsuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            Action act = () => unicoMestre.SairDaConta();
            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.UNICO_USUARIO_MESTRE_NA_CONTA);
        }

        #endregion

        #region RemoverUsuarioDaConta

        // Testes de Sucesso
        [Fact]
        public void RemoverUsuarioDaConta_DeveRemoverComSucesso_QuandoRemetenteForMestreEAlvoNaoForMestre()
        {
            var conta = CriarContaAtiva();
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            Action act = () => alvo.RemoverUsuarioDaConta(remetente);
            act.Should().NotThrow(); // Não deve lançar exceção
        }

        // Testes de Erro
        [Fact]
        public void RemoverUsuarioDaConta_DeveFalhar_SeRemetenteForOMesmoUsuario()
        {
            var conta = CriarContaAtiva();
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () =>
                remetente.RemoverUsuarioDaConta(remetente);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_TENTA_SE_EXPULSAR);
        }

        [Fact]
        public void RemoverUsuarioDaConta_DeveFalhar_SeAlvoForMestre()
        {
            var conta = CriarContaAtiva();

            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var alvoMestre = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () =>
                alvoMestre.RemoverUsuarioDaConta(remetente);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_REMOVIDO);
        }

        [Fact]
        public void RemoverUsuarioDaConta_DeveFalhar_SeRemetenteNaoForMestre()
        {
            var conta = CriarContaAtiva();

            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador);
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            Action act = () =>
                alvo.RemoverUsuarioDaConta(remetente);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact]
        public void RemoverUsuarioDaConta_DeveFalhar_SeRemetenteEstiverInativo()
        {
            var conta = CriarContaAtiva();

            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre, TipoStatusContasUsuario.Inativo);
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            Action act = () =>
                alvo.RemoverUsuarioDaConta(remetente);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
        }

        [Fact]
        public void RemoverUsuarioDaConta_DeveFalhar_SeUsuarioRemetenteNaoPertenceAContaDoAlvo()
        {
            var conta1 = CriarContaAtiva();
            var conta2 = CriarContaAtiva();

            var alvo = CriarContaUsuario(conta1, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetenteDeOutraConta = CriarContaUsuario(conta2, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () =>
                alvo.RemoverUsuarioDaConta(remetenteDeOutraConta);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        }

        #endregion
    }
}
