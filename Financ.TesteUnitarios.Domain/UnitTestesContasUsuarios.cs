using FluentAssertions;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;

namespace Financ.TesteUnitarios.Domain
{
    public class ContasUsuariosTests
    {
        private Conta CriarContaAtiva()
            => new Conta("Conta Teste");

        private Usuario CriarUsuario(string id)
            => new Usuario(id, "Nome", "Sobrenome", $"{id}@teste.com", "salt", "hash");

        private string NovoIdUsuario() => Guid.NewGuid().ToString();

        private Convite CriarConvite(TiposAcessos acesso, ContaUsuario usuarioRemetente, Usuario usuarioDestinatario, int? expiracaoContaUsuario = null)
            => new Convite(acesso, usuarioRemetente, usuarioDestinatario, expiracaoContaUsuario);

        private ContaUsuario CriarContaUsuario(Conta conta, string idUsuario, TiposAcessos acesso = TiposAcessos.Mestre, TipoStatusContasUsuario status = TipoStatusContasUsuario.Ativo)
        {
            ContaUsuario contaUsuario = new ContaUsuario(
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

            var usuario = new ContaUsuario(1, conta, CriarUsuario(NovoIdUsuario()).Id, TiposAcessos.Administrador);

            usuario.Status.Should().Be(TipoStatusContasUsuario.Ativo);
            usuario.Acesso.Should().Be(TiposAcessos.Administrador);
        }

        [Fact]
        public void Construtor_ComUsuario_DeveCriarComoMestre()
        {
            var conta = CriarContaAtiva();
            var usuarioId = CriarUsuario(NovoIdUsuario()).Id;

            var contasUsuario = new ContaUsuario(conta, usuarioId);

            contasUsuario.Acesso.Should().Be(TiposAcessos.Mestre);
            contasUsuario.Status.Should().Be(TipoStatusContasUsuario.Ativo);
        }

        [Fact]
        public void Construtor_Convite_DeveCriarComAcessoAdministrador_QuandoLimiteMestresAtingido()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            var convite = CriarConvite(TiposAcessos.Mestre, mestre1, CriarUsuario(NovoIdUsuario()));

            var mestre2 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            var contasUsuario = new ContaUsuario(convite);

            contasUsuario.Acesso.Should().Be(TiposAcessos.Administrador);
            convite.Observacao.Should().Contain(MensagensContasUsuarios.MAX_MESTRES_CONVERTE_PARA_ADMIN);
        }

        [Fact]
        public void Construtor_Convite_DeveCriarComAcessoCorreto_QuandoNaoAtingeLimiteMestres()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var convite = CriarConvite(TiposAcessos.Visualizador, mestre1, usuarioDestinatario);

            var contasUsuario = new ContaUsuario(convite);

            contasUsuario.Acesso.Should().Be(TiposAcessos.Visualizador);
            contasUsuario.Status.Should().Be(TipoStatusContasUsuario.Ativo);
            convite.Observacao.Should().BeNull();
        }

        [Fact]
        public void Construtor_Convite_DeveDefinirExpiracaoCorretamente()
        {
            var conta = CriarContaAtiva();
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var destinatario = CriarUsuario(NovoIdUsuario());
            int minutosExpiracao = 30;

            var convite = CriarConvite(TiposAcessos.Visualizador, remetente, destinatario, minutosExpiracao);
            var contasUsuario = new ContaUsuario(convite);

            contasUsuario.Expiracao.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(minutosExpiracao), TimeSpan.FromSeconds(5));
        }

        // Testes de Erro
        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Construtor_ComIdInvalido_DeveLancarExcecao(int idInvalido)
        {
            var conta = CriarContaAtiva();

            Action act = () =>
                new ContaUsuario(idInvalido, conta, "user-1", TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensBase.ID_IGUAL_MENOR_ZERO);
        }

        [Fact]
        public void Construtor_ComContaNula_DeveLancarExcecao()
        {
            Action act = () =>
                new ContaUsuario(1, null, CriarUsuario(NovoIdUsuario()).Id, TiposAcessos.Administrador);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.CONTA_NAO_PODE_SER_NULA);
        }

        [Fact]
        public void Construtor_ComContaInativa_DeveLancarExcecao()
        {
            var conta = new Conta("Conta Teste");
            var usuarioRemetente = CriarContaUsuario(conta, NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var convite = CriarConvite(TiposAcessos.Mestre, usuarioRemetente, usuarioDestinatario);

            // Simula a conta se tornando inativa ANTES da criação do ContasUsuarios pelo convite
            conta.AtualizaConta(usuarioRemetente, null, TiposStatusContas.Inativo);

            Action act = () =>
                new ContaUsuario(convite);

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
                new ContaUsuario(1, conta, idUsuario, TiposAcessos.Administrador);

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
                new ContaUsuario(1, conta, NovoIdUsuario(), TiposAcessos.Administrador, statusInvalido);

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
                new ContaUsuario(1, conta, NovoIdUsuario(), acessoInvalido);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_INVALIDO);
        }

        [Fact]
        public void Construtor_ConviteNulo_DeveLancarExcecao()
        {
            Action act = () => new ContaUsuario(null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.CONVITE_NAO_PODE_SER_NULO);
        }

        #endregion

        #region AtualizaOutraContaUsuario

        // Testes de Sucesso
        [Fact]
        public void Atualiza_FluxoValido_DeveAtualizarAcessoEStatus()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

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
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            alvo.Acesso.Should().Be(TiposAcessos.Administrador);
            alvo.Status.Should().Be(TipoStatusContasUsuario.Ativo); // Status deve permanecer o mesmo
        }

        [Fact]
        public void Atualiza_ApenasStatus_DeveAtualizarCorretamente()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            alvo.AtualizaOutraContaUsuario(remetente, null, TipoStatusContasUsuario.Bloqueado);

            alvo.Acesso.Should().Be(TiposAcessos.Visualizador); // Acesso deve permanecer o mesmo
            alvo.Status.Should().Be(TipoStatusContasUsuario.Bloqueado);
        }

        [Fact]
        public void Atualiza_DeveDefinirExpiracao_QuandoParametroExpiracaoFornecido()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            int minutosExpiracao = 60;

            alvo.AtualizaOutraContaUsuario(remetente, null, null, minutosExpiracao);

            alvo.Expiracao.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(minutosExpiracao), TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Atualiza_DeveExpirarConta_QuandoParametroExpiradoForTrue()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            alvo.AtualizaOutraContaUsuario(remetente, null, null, null, true);

            alvo.Expiracao.Should().BeBefore(DateTime.UtcNow);
        }

        [Fact]
        public void Atualiza_DeveRemoverExpiracao_QuandoParametroExpiradoForFalse()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            // Define uma expiração inicial
            alvo.AtualizaOutraContaUsuario(remetente, null, null, 60);
            alvo.Expiracao.Should().NotBeNull();

            // Remove a expiração
            alvo.AtualizaOutraContaUsuario(remetente, null, null, null, false);

            alvo.Expiracao.Should().BeNull();
        }

        // Testes de Erro
        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteForOMesmoUsuario()
        {
            var conta = CriarContaAtiva();
            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () =>
                usuario.AtualizaOutraContaUsuario(usuario, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_TENTA_SE_ATUALIZAR);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteNaoForMestre()
        {
            var conta = CriarContaAtiva();

            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeRemetenteEstiverInativo()
        {
            var conta = CriarContaAtiva();

            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre, TipoStatusContasUsuario.Inativo);

            Action act = () =>
                alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
        }

       

        [Fact]
        public void Atualiza_DeveFalhar_SeUsuarioAlvoForMestreEStatusDiferenteDeAtivo()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () => alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Mestre, TipoStatusContasUsuario.Inativo);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ATUALIZA_PARA_USUARIO_MESTRE_DIFERENTE_DE_ATIVO);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeConflitoAoExpirar()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () => alvo.AtualizaOutraContaUsuario(remetente, null, null, 30, true);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.CONFLITO_AO_EXPIRAR);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeTempoMinimoExpiracaoInvalido()
        {
            var conta = CriarContaAtiva();
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () => alvo.AtualizaOutraContaUsuario(remetente, null, null, 10); // Menos de 15 minutos

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.TEMPO_MIN_EXPIRACAO);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeUsuarioMestreTentarAtualizarAcessoParaMestreELimiteAtingido()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var mestre2 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            Action act = () => alvo.AtualizaOutraContaUsuario(mestre1, TiposAcessos.Mestre, null);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensBase.LIMITE_USUARIOS_MESTRES);
        }

        [Fact]
        public void Atualiza_DeveFalhar_SeUsuarioMestreTentarAtualizarAcessoParaMestreComTempoLimite()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var alvo = new ContaUsuario(CriarConvite(TiposAcessos.Administrador,mestre1,CriarUsuario(NovoIdUsuario()),null));

            Action act = () => alvo.AtualizaOutraContaUsuario(mestre1, TiposAcessos.Mestre, null, 30);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_MESTRE_COM_TEMPO_LIMITE_JA_DEFINIDO);
        }

        #endregion

        #region SairDaConta
       

        [Fact]
        public void SairDaConta_DeveRemoverUsuarioEInativarConta_SeForUltimoUsuario()
        {
            var conta = CriarContaAtiva();
            var mestre = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            mestre.SairDaConta();
            conta.SairDaConta(mestre);

            conta.ContaUsuarios.Should().BeEmpty();
            conta.Status.Should().Be(TiposStatusContas.Inativo);
        }

        // Testes de Erro
        [Fact]
        public void SairDaConta_DeveFalhar_SeUnicoUsuarioMestreTentarSairComOutrosUsuariosNaoMestres()
        {
            var conta = CriarContaAtiva();
            var mestre = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var admin = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador);

            Action act = () => mestre.SairDaConta();

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.UNICO_USUARIO_MESTRE_NA_CONTA);
        }

        [Fact]
        public void SairDaConta_DeveFalhar_SeUsuarioPossuiConvitesEmAndamento()
        {
            var conta = CriarContaAtiva();
            var mestre = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var destinatario = CriarUsuario(NovoIdUsuario());
            var convite = CriarConvite(TiposAcessos.Visualizador, mestre, destinatario);
            conta.AddConvite(convite);

            Action act = () => mestre.SairDaConta();

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_POSSUI_CONVITES_EM_ANDAMENTO);
        }

        #endregion

        #region RemoverUsuarioDaConta

        // Testes de Sucesso
        [Fact]
        public void RemoverUsuarioDaConta_DeveRemoverUsuarioComSucesso()
        {
            var conta = CriarContaAtiva();
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            alvo.RemoverUsuarioDaConta(remetente);
            conta.SairDaConta(alvo); // A remoção da lista é feita na Conta, não no ContasUsuarios

            conta.ContaUsuarios.Should().NotContain(alvo);
        }

        // Testes de Erro
        [Fact]
        public void RemoverUsuarioDaConta_DeveFalhar_SeRemetenteNaoForMestreAtivoDaConta()
        {
            var conta = CriarContaAtiva();
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Administrador);
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            Action act = () => alvo.RemoverUsuarioDaConta(remetente);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact]
        public void RemoverUsuarioDaConta_DeveFalhar_SeRemetenteForOMesmoUsuario()
        {
            var conta = CriarContaAtiva();
            var usuario = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () => usuario.RemoverUsuarioDaConta(usuario);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_TENTA_SE_EXPULSAR);
        }

        [Fact]
        public void RemoverUsuarioDaConta_DeveFalhar_SeUsuarioAlvoForMestre()
        {
            var conta = CriarContaAtiva();
            var remetente = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var alvo = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            Action act = () => alvo.RemoverUsuarioDaConta(remetente);

            act.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_REMOVIDO);
        }

        #endregion

        #region ValidaPermissoeNaConta

        [Fact]
        public void ValidaPermissoeNaConta_DeveRetornarTrue_SeAcessoMestreNaoAtingiuLimite()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            var contasUsuario = new ContaUsuario(1, conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            contasUsuario.ValidaPermissoeNaConta(TiposAcessos.Mestre).Should().BeTrue();
        }

        [Fact]
        public void ValidaPermissoeNaConta_DeveRetornarFalse_SeAcessoMestreAtingiuLimite()
        {
            var conta = CriarContaAtiva();
            var mestre1 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);
            var mestre2 = CriarContaUsuario(conta, NovoIdUsuario(), TiposAcessos.Mestre);

            var contasUsuario = new ContaUsuario(1, conta, NovoIdUsuario(), TiposAcessos.Visualizador);

            contasUsuario.ValidaPermissoeNaConta(TiposAcessos.Mestre).Should().BeFalse();
        }

        #endregion

        #region ValidaUsuarioMestre

        [Theory]
        [InlineData(TiposAcessos.Mestre, true)]
        [InlineData(TiposAcessos.Administrador, false)]
        [InlineData(TiposAcessos.Visualizador, false)]
        public void ValidaUsuarioMestre_DeveRetornarCorretamente(TiposAcessos acesso, bool esperado)
        {
            var conta = CriarContaAtiva();
            var contasUsuario = CriarContaUsuario(conta, NovoIdUsuario(), acesso);

            contasUsuario.ValidaUsuarioMestre(acesso).Should().Be(esperado);
        }

        #endregion

        #region ExpiracaoPorAcesso

        [Theory]
        [InlineData(TiposAcessos.Mestre, true)]
        [InlineData(TiposAcessos.Administrador, false)]
        [InlineData(TiposAcessos.Visualizador, false)]
        public void ExpiracaoPorAcesso_DeveRetornarCorretamente(TiposAcessos acesso, bool esperado)
        {
            var conta = CriarContaAtiva();
            var contasUsuario = CriarContaUsuario(conta, NovoIdUsuario(), acesso);

            contasUsuario.ExpiracaoPorAcesso(acesso).Should().Be(esperado);
        }

        #endregion

        #region ValidaExpiracao

        [Theory]
        [InlineData(10, true)] // Menor que 15
        [InlineData(15, false)] // Igual a 15
        [InlineData(20, false)] // Maior que 15
        public void ValidaExpiracao_DeveRetornarCorretamente(int minutos, bool esperado)
        {
            var conta = CriarContaAtiva();
            var contasUsuario = CriarContaUsuario(conta, NovoIdUsuario());

            contasUsuario.ValidaExpiracao(minutos).Should().Be(esperado);
        }

        #endregion
    }
}
