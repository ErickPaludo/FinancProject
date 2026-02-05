using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesContasUsuarios
    {
        // Helper para criar uma conta válida (Dummy) para os testes não ficarem repetitivos
        // Assumindo que o construtor padrão cria uma conta com Status Ativo
        private Conta CriarContaPadrao()
        {
            return new Conta("Conta Teste", false, null, null, false, null);
        }

        // ====================================================================
        // TESTES DE CRIAÇÃO (CONSTRUTORES)
        // ====================================================================

        [Theory(DisplayName = "Construtor: IDs inválidos devem gerar divergência")]
        [InlineData(0)]
        [InlineData(-10)]
        public void Construtor_IdInvalido_DeveLancarExcecao(int idInvalido)
        {
            // Arrange
            var conta = CriarContaPadrao();
            var idUsuario = Guid.NewGuid().ToString();

            // Act
            Action action = () => new ContasUsuarios(idInvalido, conta, idUsuario, TiposAcessos.Visualizador, TiposStatus.Ativo);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensBase.ID_IGUAL_MENOR_ZERO);
        }

        [Fact(DisplayName = "Construtor: Conta nula deve gerar divergência")]
        public void Construtor_ContaNula_DeveLancarExcecao()
        {
            // Act
            Action action = () => new ContasUsuarios(null, "guid-user", TiposAcessos.Visualizador, TiposStatus.Ativo);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.CONTA_NAO_PODE_SER_NULA);
        }

        [Theory(DisplayName = "Construtor: IdUsuario vazio ou nulo deve gerar divergência")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Construtor_IdUsuarioVazio_DeveLancarExcecao(string idUsuarioInvalido)
        {
            // Arrange
            var conta = CriarContaPadrao();

            // Act
            Action action = () => new ContasUsuarios(conta, idUsuarioInvalido, TiposAcessos.Visualizador, TiposStatus.Ativo);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.IDUSUARIO_INVALIDO);
        }

        [Fact(DisplayName = "Construtor: Enums inválidos devem gerar divergência")]
        public void Construtor_EnumsInvalidos_DeveLancarExcecao()
        {
            // Arrange
            var conta = CriarContaPadrao();
            var idUsuario = Guid.NewGuid().ToString();

            // Act - Acesso Inválido
            Action actionAcesso = () => new ContasUsuarios(conta, idUsuario, (TiposAcessos)999, TiposStatus.Ativo);
            actionAcesso.Should().Throw<ContasUsuariosValidacao>().WithMessage(MensagensContasUsuarios.ACESSO_INVALIDO);

            // Act - Status Inválido
            Action actionStatus = () => new ContasUsuarios(conta, idUsuario, TiposAcessos.Visualizador, (TiposStatus)999);
            actionStatus.Should().Throw<ContasUsuariosValidacao>().WithMessage(MensagensBase.STATUS_INVALIDO);
        }

        [Fact(DisplayName = "Construtor: Criação válida com todos parâmetros")]
        public void Construtor_Valido_NaoDeveGerarExcecao()
        {
            // Arrange
            var conta = CriarContaPadrao();
            var idUsuario = Guid.NewGuid().ToString();

            // Act
            Action action = () => new ContasUsuarios(1, conta, idUsuario, TiposAcessos.Administrador, TiposStatus.Ativo);

            // Assert
            action.Should().NotThrow<ContasUsuariosValidacao>();
        }

        [Fact(DisplayName = "Construtor: Criação simplificada define padrão Mestre/Ativo")]
        public void Construtor_Simplificado_DeveCriarComoMestreEAtivo()
        {
            // Arrange
            var conta = CriarContaPadrao();
            var idUsuario = Guid.NewGuid().ToString();

            // Act
            var usuarioConta = new ContasUsuarios(conta, idUsuario);

            // Assert
            usuarioConta.Acesso.Should().Be(TiposAcessos.Mestre);
            usuarioConta.Status.Should().Be(TiposStatus.Ativo);
        }

        // ====================================================================
        // TESTES DE ATUALIZAÇÃO (Regras de Negócio)
        // ====================================================================

        [Fact(DisplayName = "Atualização: Falha se o remetente tentar atualizar a si mesmo")]
        public void Atualiza_RemetenteIgualUsuario_DeveLancarAcessoNegado()
        {
            // Arrange
            var conta = CriarContaPadrao();
            var idMesmoUsuario = Guid.NewGuid().ToString();

            // O remetente e o objeto sendo editado possuem o mesmo ID de Usuário
            var usuarioAlvo = new ContasUsuarios(1, conta, idMesmoUsuario, TiposAcessos.Mestre, TiposStatus.Ativo);
            var remetente = new ContasUsuarios(1, conta, idMesmoUsuario, TiposAcessos.Mestre, TiposStatus.Ativo);

            // Act
            Action action = () => usuarioAlvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Visualizador, null);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact(DisplayName = "Atualização: Falha se remetente NÃO for Mestre")]
        public void Atualiza_RemetenteNaoMestre_DeveLancarAcessoNegado()
        {
            // Regra do código: if (contasUsuarioRemetente.Acesso != TiposAcessos.Mestre)

            // Arrange
            var conta = CriarContaPadrao();
            var alvo = new ContasUsuarios(1, conta, "id-alvo", TiposAcessos.Visualizador, TiposStatus.Ativo);

            // Remetente é apenas Administrador
            var remetenteAdmin = new ContasUsuarios(2, conta, "id-admin", TiposAcessos.Administrador, TiposStatus.Ativo);

            // Act
            Action action = () => alvo.AtualizaOutraContaUsuario(remetenteAdmin, TiposAcessos.Administrador, null);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO);
        }

        [Fact(DisplayName = "Atualização: Falha se remetente estiver com Status diferente de Ativo")]
        public void Atualiza_RemetenteInativo_DeveLancarAcessoNegadoPorStatus()
        {
            // Arrange
            var conta = CriarContaPadrao();
            var alvo = new ContasUsuarios(1, conta, "id-alvo", TiposAcessos.Visualizador, TiposStatus.Ativo);

            // Remetente é Mestre, mas está Bloqueado
            var remetenteBloqueado = new ContasUsuarios(2, conta, "id-mestre", TiposAcessos.Mestre, TiposStatus.Bloqueado);

            // Act
            Action action = () => alvo.AtualizaOutraContaUsuario(remetenteBloqueado, TiposAcessos.Administrador, null);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_NEGADO_POR_STATUS);
        }

        [Fact(DisplayName = "Atualização: Falha ao tentar alterar um usuário Mestre")]
        public void Atualiza_AlvoEhMestre_DeveImpedirAlteracao()
        {
            // Regra: if (Acesso == TiposAcessos.Mestre) -> USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO

            // Arrange
            var conta = CriarContaPadrao();
            // Alvo já é Mestre
            var alvoMestre = new ContasUsuarios(1, conta, "id-alvo-mestre", TiposAcessos.Mestre, TiposStatus.Ativo);
            var remetente = new ContasUsuarios(2, conta, "id-remetente", TiposAcessos.Mestre, TiposStatus.Ativo);

            // Act
            Action action = () => alvoMestre.AtualizaOutraContaUsuario(remetente, TiposAcessos.Visualizador, null);

            // Assert
            action.Should().Throw<ContasUsuariosValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_MESTRE_NAO_PODE_SER_ATUALIZADO);
        }

        [Fact(DisplayName = "Atualização: Sucesso quando Mestre altera Visualizador")]
        public void Atualiza_FluxoSucesso_DeveAtualizarPropriedades()
        {
            // Arrange
            var conta = CriarContaPadrao();
            var alvo = new ContasUsuarios(1, conta, "id-alvo", TiposAcessos.Visualizador, TiposStatus.Ativo);
            var remetente = new ContasUsuarios(2, conta, "id-remetente", TiposAcessos.Mestre, TiposStatus.Ativo);

            // Act
            // Mudando acesso para Admin e status para Bloqueado
            alvo.AtualizaOutraContaUsuario(remetente, TiposAcessos.Administrador, TiposStatus.Bloqueado);

            // Assert
            alvo.Acesso.Should().Be(TiposAcessos.Administrador);
            alvo.Status.Should().Be(TiposStatus.Bloqueado);
        }
    }
}
