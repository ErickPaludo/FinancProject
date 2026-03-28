using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using FluentAssertions;
using Xunit;
using System;
using System.Linq;
using System.Reflection;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesConvites
    {
        private Conta CriarContaAtiva(int id = 1)
            => new Conta(id, "Conta Teste");

        private Usuario CriarUsuario(string id)
            => new Usuario(id, "Nome", "Sobrenome", $"{id}@teste.com", "salt", "hash");

        private ContasUsuarios CriarContaUsuarioMestre(Conta conta, string idUsuario, TipoStatusContasUsuario status = TipoStatusContasUsuario.Ativo)
        {
            ContasUsuarios contaUsuario = new ContasUsuarios(
                conta.ContaUsuarios.Count() + 1,
                conta,
                idUsuario,
                TiposAcessos.Mestre, status);
            conta.AddUsuario(contaUsuario);
            return contaUsuario;
        }

        private ContasUsuarios CriarContaUsuarioAdministrador(Conta conta, string idUsuario, TipoStatusContasUsuario status = TipoStatusContasUsuario.Ativo)
        {
            ContasUsuarios contaUsuario = new ContasUsuarios(
                conta.ContaUsuarios.Count() + 1,
                conta,
                idUsuario,
                TiposAcessos.Administrador, status);
            conta.AddUsuario(contaUsuario);
            return contaUsuario;
        }

        private string NovoIdUsuario() => Guid.NewGuid().ToString();

        #region Construtor Convites

        // Testes de Sucesso
        [Fact(DisplayName = "Deve criar convite quando dados são válidos")]
        public void CriarConvite_DadosValidos_NaoDeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().NotThrow();
        }

        [Fact(DisplayName = "Deve criar convite com tempo de expiração definido")]
        public void CriarConvite_ComExpiracaoContaUsuario_DeveDefinirExpiracaoCorretamente()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);
            int minutosExpiracao = 30;

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario,
                minutosExpiracao);

            convite.ExpiracaoContaUsuario.Should().Be(minutosExpiracao);
            convite.Expiracao.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(5)); // Expiracao do convite é fixa em 7 dias
        }

        // Testes de Erro
        [Fact(DisplayName = "Construtor Convites - Deve lançar exceção para acesso inválido")]
        public void CriarConvite_ComAcessoInvalido_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);
            TiposAcessos acessoInvalido = (TiposAcessos)999;

            Action action = () => new Convites(
                acessoInvalido,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensContasUsuarios.ACESSO_INVALIDO);
        }

        [Fact(DisplayName = "Construtor Convites - Deve lançar exceção para usuário remetente nulo")]
        public void CriarConvite_ComUsuarioRemetenteNulo_DeveLancarExcecao()
        {
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                null,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.USUARIO_DESTINATARIO_NAO_ENCONTRADO);
        }

        [Fact(DisplayName = "Construtor Convites - Deve lançar exceção para usuário destinatário nulo")]
        public void CriarConvite_ComUsuarioDestinatarioNulo_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                null);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.USUARIO_DESTINATARIO_NAO_ENCONTRADO);
        }

        [Fact(DisplayName = "Não deve permitir convite se usuário não for mestre")]
        public void CriarConvite_UsuarioNaoMestre_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());

            var contaUsuarioRemetente = new ContasUsuarios(
                1,
                conta,
                usuarioRemetente.Id,
                TiposAcessos.Administrador);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.USUARIO_SEM_PERMISSAO);
        }

        [Fact(DisplayName = "Não deve permitir convite se conta não for ativa")]
        public void CriarConvite_ContaInativa_DeveLancarExcecao()
        {
            var conta = new Conta(1, "Conta Teste");

            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            // Simula a conta se tornando inativa ANTES da criação do convite
            // Para simular a conta inativa, precisamos de um usuário mestre para chamar AtualizaConta
            var outroMestre = CriarContaUsuarioMestre(conta, Guid.NewGuid().ToString());
            conta.AtualizaConta(outroMestre, null, TiposStatusContas.Inativo);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
              .WithMessage(MensagensContas.CONTA_INATIVA);
        }

        [Theory(DisplayName = "Não deve permitir convite se o usuário remetente não for ativo")]
        [InlineData(TipoStatusContasUsuario.Inativo)]
        [InlineData(TipoStatusContasUsuario.Bloqueado)]
        public void CriarConvite_UsuarioRemetenteInativo_DeveLancarExcecao(TipoStatusContasUsuario status)
        {
            var conta = new Conta(1, "Conta Teste");

            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id, status);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
              .WithMessage(MensagensConvite.USUARIO_CONTA_REMETENTE_INATIVO);
        }

        [Fact(DisplayName = "Não deve permitir convite se o usuário destinatário já pertence à conta")]
        public void CriarConvite_UsuarioDestinatarioJaPertenceAConta_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);
            CriarContaUsuarioAdministrador(conta, usuarioDestinatario.Id); // Adiciona o destinatário à conta

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.USUARIO_JA_PERTENCE_A_CONTA);
        }

        [Fact(DisplayName = "Não deve permitir convite se já existe um convite em andamento para o destinatário")]
        public void CriarConvite_ConviteEmAndamentoParaDestinatario_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            // Cria um convite em andamento
            var conviteExistente = new Convites(TiposAcessos.Visualizador, contaUsuarioRemetente, usuarioDestinatario);
            conta.AddConvite(conviteExistente);

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_EM_ANDAMENTO);
        }

        [Fact(DisplayName = "Não pode enviar convite para usuário Mestre se existir 2 usuarios mestres cadastrados.")]
        public void Limite_Mestres_Cadastrados_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();

            var usuario1 = CriarUsuario(NovoIdUsuario());
            var usuario2 = CriarUsuario(NovoIdUsuario());
            var usuario3 = CriarUsuario(NovoIdUsuario());

            // Adiciona dois usuários mestre à conta
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuario1.Id);
            CriarContaUsuarioMestre(conta, usuario2.Id);

            Action action = () => new Convites(
                TiposAcessos.Mestre,
                contaUsuarioRemetente,
                usuario3);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensBase.LIMITE_USUARIOS_MESTRES);
        }

        [Fact(DisplayName = "Não deve permitir convite com tempo de expiração para Mestre")]
        public void CriarConvite_MestreComExpiracao_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);
            int minutosExpiracao = 30;

            Action action = () => new Convites(
                TiposAcessos.Mestre,
                contaUsuarioRemetente,
                usuarioDestinatario,
                minutosExpiracao);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensContasUsuarios.MESTRE_NAO_POSSUI_TEMPO_LIMITE);
        }

        [Fact(DisplayName = "Não deve permitir convite com tempo de expiração menor que o mínimo")]
        public void CriarConvite_ExpiracaoMenorQueMinimo_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);
            int minutosExpiracao = 10; // Menor que 15

            Action action = () => new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario,
                minutosExpiracao);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensContasUsuarios.TEMPO_MIN_EXPIRACAO);
        }

        #endregion

        #region AceitaConvite

        // Testes de Sucesso
        [Fact(DisplayName = "Aceitar convite deve funcionar quando válido")]
        public void AceitarConvite_Valido_DeveAlterarEstadoParaAceito()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            convite.AceitaConvite(true);

            convite.Aceito.Should().BeTrue();
        }

        [Fact(DisplayName = "Rejeitar convite deve funcionar quando válido")]
        public void AceitarConvite_Valido_DeveAlterarEstadoParaRejeitado()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            convite.AceitaConvite(false);

            convite.Aceito.Should().BeFalse();
        }

        // Testes de Erro
        [Fact(DisplayName = "Aceitar convite - Deve lançar exceção se convite já foi aceito ou rejeitado")]
        public void AceitarConvite_JaVisualizado_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            convite.AceitaConvite(true); // Aceita o convite primeiro

            Action action = () => convite.AceitaConvite(false); // Tenta rejeitar novamente

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_JA_VISUALIZADO + "aceito");
        }

        [Fact(DisplayName = "Aceitar convite - Deve lançar exceção se convite expirou")]
        public void AceitarConvite_Expirado_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            // Usa Reflection para manipular a data de expiração para o passado
            typeof(Convites).GetProperty(nameof(Convites.Expiracao), BindingFlags.Public | BindingFlags.Instance)
                .SetValue(convite, DateTime.UtcNow.AddDays(-1));

            Action action = () => convite.AceitaConvite(true);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_EXPIRADO);
        }

        #endregion

        #region InsereObservacao

        [Fact(DisplayName = "InsereObservacao - Deve definir a observação corretamente")]
        public void InsereObservacao_DeveDefinirObservacao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            string observacaoEsperada = "Esta é uma observação de teste.";
            convite.InsereObservacao(observacaoEsperada);

            convite.Observacao.Should().Be(observacaoEsperada);
        }

        #endregion

        #region RevogaConvite

        [Fact(DisplayName = "RevogaConvite - Deve revogar convite com sucesso")]
        public void RevogaConvite_ComSucesso()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            Action action = () => convite.RevogaConvite(usuarioRemetente.Id);

            action.Should().NotThrow();
            // A revogação não altera o estado 'Aceito', apenas valida se pode ser revogado.
            // Para verificar a revogação, seria necessário um mecanismo externo (ex: um repositório que remove o convite).
            // O teste aqui foca na validação interna do método.
        }

        [Fact(DisplayName = "RevogaConvite - Deve lançar exceção se usuário remetente for diferente")]
        public void RevogaConvite_UsuarioRemetenteDiferente_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            string outroIdUsuario = NovoIdUsuario();

            Action action = () => convite.RevogaConvite(outroIdUsuario);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_EXPIRADO); // Mensagem de erro inadequada, mas é a definida na classe
        }

        [Fact(DisplayName = "RevogaConvite - Deve lançar exceção se convite já foi aceito ou rejeitado")]
        public void RevogaConvite_JaVisualizado_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            convite.AceitaConvite(true); // Aceita o convite primeiro

            Action action = () => convite.RevogaConvite(usuarioRemetente.Id);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_JA_VISUALIZADO + "aceito");
        }

        [Fact(DisplayName = "RevogaConvite - Deve lançar exceção se convite expirou")]
        public void RevogaConvite_Expirado_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            // Usa Reflection para manipular a data de expiração para o passado
            typeof(Convites).GetProperty(nameof(Convites.Expiracao), BindingFlags.Public | BindingFlags.Instance)
                .SetValue(convite, DateTime.UtcNow.AddDays(-1));

            Action action = () => convite.RevogaConvite(usuarioRemetente.Id);

            action.Should().Throw<ConvitesValidacao>()
                .WithMessage(MensagensConvite.CONVITE_EXPIRADO);
        }

        #endregion

        #region Conta - Interações com Convites e ContasUsuarios

        [Fact(DisplayName = "AddConvite - Deve adicionar convite à coleção da conta")]
        public void AddConvite_DeveAdicionarConviteAConta()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(
                TiposAcessos.Administrador,
                contaUsuarioRemetente,
                usuarioDestinatario);

            conta.AddConvite(convite);

            conta.Convites.Should().Contain(convite);
        }

        [Fact(DisplayName = "SairDaConta - Deve lançar exceção se contaUsuario for nulo")]
        public void SairDaConta_ContaUsuarioNulo_DeveLancarExcecao()
        {
            var conta = CriarContaAtiva();

            Action action = () => conta.SairDaConta(null);

            action.Should().Throw<ContasValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        }

        [Fact(DisplayName = "SairDaConta - Deve lançar exceção se contaUsuario pertencer a outra conta")]
        public void SairDaConta_ContaUsuarioDeOutraConta_DeveLancarExcecao()
        {
            var conta1 = CriarContaAtiva(1);
            var conta2 = CriarContaAtiva(2);
            var usuario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioDeOutraConta = CriarContaUsuarioMestre(conta2, usuario.Id);

            Action action = () => conta1.SairDaConta(contaUsuarioDeOutraConta);

            action.Should().Throw<ContasValidacao>()
                .WithMessage(MensagensContasUsuarios.USUARIO_NAO_PERTENCE_A_CONTA);
        }

        [Fact(DisplayName = "SairDaConta - Deve remover o usuário da coleção da conta")]
        public void SairDaConta_DeveRemoverUsuarioDaColecao()
        {
            var conta = CriarContaAtiva();
            var usuario1 = CriarUsuario(NovoIdUsuario());
            var usuario2 = CriarUsuario(NovoIdUsuario());
            var contaUsuario1 = CriarContaUsuarioMestre(conta, usuario1.Id);
            var contaUsuario2 = CriarContaUsuarioAdministrador(conta, usuario2.Id);

            conta.SairDaConta(contaUsuario2);

            conta.ContaUsuarios.Should().NotContain(contaUsuario2);
            conta.ContaUsuarios.Should().Contain(contaUsuario1);
        }

        [Fact(DisplayName = "SairDaConta - Deve tornar a conta inativa se for o último usuário a sair")]
        public void SairDaConta_UltimoUsuario_DeveTornarContaInativa()
        {
            var conta = CriarContaAtiva();
            var usuario = CriarUsuario(NovoIdUsuario());
            var contaUsuario = CriarContaUsuarioMestre(conta, usuario.Id);

            conta.SairDaConta(contaUsuario);

            conta.Status.Should().Be(TiposStatusContas.Inativo);
        }

        [Fact(DisplayName = "ConviteEmAndamento - Deve retornar true se houver convite em andamento")]
        public void ConviteEmAndamento_ComConviteAtivo_DeveRetornarTrue()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(TiposAcessos.Visualizador, contaUsuarioRemetente, usuarioDestinatario);
            conta.AddConvite(convite);

            conta.ConviteEmAndamento(usuarioDestinatario.Id).Should().BeTrue();
        }

        [Fact(DisplayName = "ConviteEmAndamento - Deve retornar false se não houver convite em andamento")]
        public void ConviteEmAndamento_SemConviteAtivo_DeveRetornarFalse()
        {
            var conta = CriarContaAtiva();
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());

            conta.ConviteEmAndamento(usuarioDestinatario.Id).Should().BeFalse();
        }

        [Fact(DisplayName = "ConviteEmAndamento - Deve retornar false se convite expirou")]
        public void ConviteEmAndamento_ConviteExpirado_DeveRetornarFalse()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(TiposAcessos.Visualizador, contaUsuarioRemetente, usuarioDestinatario);
            // Usa Reflection para manipular a data de expiração para o passado
            typeof(Convites).GetProperty(nameof(Convites.Expiracao), BindingFlags.Public | BindingFlags.Instance)
                .SetValue(convite, DateTime.UtcNow.AddDays(-1));
            conta.AddConvite(convite);

            conta.ConviteEmAndamento(usuarioDestinatario.Id).Should().BeFalse();
        }

        [Fact(DisplayName = "ConviteEmAndamento - Deve retornar false se convite foi aceito")]
        public void ConviteEmAndamento_ConviteAceito_DeveRetornarFalse()
        {
            var conta = CriarContaAtiva();
            var usuarioRemetente = CriarUsuario(NovoIdUsuario());
            var usuarioDestinatario = CriarUsuario(NovoIdUsuario());
            var contaUsuarioRemetente = CriarContaUsuarioMestre(conta, usuarioRemetente.Id);

            var convite = new Convites(TiposAcessos.Visualizador, contaUsuarioRemetente, usuarioDestinatario);
            convite.AceitaConvite(true);
            conta.AddConvite(convite);

            conta.ConviteEmAndamento(usuarioDestinatario.Id).Should().BeFalse();
        }

        [Fact(DisplayName = "UsuarioPertenceConta - Deve retornar true se usuário pertence à conta")]
        public void UsuarioPertenceConta_UsuarioExiste_DeveRetornarTrue()
        {
            var conta = CriarContaAtiva();
            var usuario = CriarUsuario(NovoIdUsuario());
            CriarContaUsuarioMestre(conta, usuario.Id);

            conta.UsuarioPertenceConta(usuario.Id).Should().BeTrue();
        }

        [Fact(DisplayName = "UsuarioPertenceConta - Deve retornar false se usuário não pertence à conta")]
        public void UsuarioPertenceConta_UsuarioNaoExiste_DeveRetornarFalse()
        {
            var conta = CriarContaAtiva();
            var usuario = CriarUsuario(NovoIdUsuario());

            conta.UsuarioPertenceConta(usuario.Id).Should().BeFalse();
        }

        #endregion
    }
}
