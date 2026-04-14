using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesMovimentacoes
    {
        private string NovoIdUsuario() => Guid.NewGuid().ToString();

        private Conta CriarContaValida(int id = 1)
           => new Conta(id, "Conta Teste", "#FFFFFF");

        private Usuario CriarUsuario(string id)
           => new Usuario(id, "Nome", "Sobrenome", $"{id}@teste.com", "salt", "hash");

        private ContaUsuario CriarContaUsuario(Conta conta, Usuario usuario, TiposAcessos acesso, TipoStatusContasUsuario status = TipoStatusContasUsuario.Ativo)
              => new ContaUsuario(
                  1,
                  conta,
                  usuario.Id,
                  acesso,
                  status);

        private ContaUsuario CriarContaUsuarioValida(TipoStatusContasUsuario status = TipoStatusContasUsuario.Ativo, TiposAcessos acesso = TiposAcessos.Administrador,bool criaConta = true)
        {
            var usuario = CriarUsuario(NovoIdUsuario());
            return  CriarContaUsuario(criaConta ? CriarContaValida() : null, usuario, acesso, status);
        }

        private Categoria CriarCategoriaValida(Conta conta)
        {
            return new Categoria(conta, "Categoria Teste", "#FFFFFF");
        }

        [Fact]
        public void CriarMovimentacao_ComParametrosValidos_DeveSerBemSucedido()
        {
            // Arrange
            var contaUsuario = CriarContaUsuarioValida();
            var categoria = CriarCategoriaValida(contaUsuario.Conta);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Salário";
            var observacao = "Recebimento mensal";
            var dthrReg = DateTime.UtcNow;

            // Act
            var movimentacao = new Movimentacao( tipo, contaUsuario, categoria, valor, titulo, observacao, dthrReg,null);

            // Assert
            movimentacao.Should().NotBeNull();
            movimentacao.Tipo.Should().Be(tipo);
            movimentacao.Valor.Should().Be(valor);
            movimentacao.Titulo.Should().Be(titulo);
            movimentacao.Observacao.Should().Be(observacao);
            movimentacao.DthrMovimentacao.Should().Be(dthrReg);
        }

        [Theory]
        [InlineData(TipoMovimentacao.Entrada - 1)] // Valor inválido para TipoMovimentacao
        [InlineData(TipoMovimentacao.Saida + 1)] // Valor inválido para TipoMovimentacao
        public void CriarMovimentacao_ComTipoMovimentacaoInvalido_DeveLancarExcecao(TipoMovimentacao tipoInvalido)
        {
            // Arrange
            var contaUsuario = CriarContaUsuarioValida();
            var categoria = CriarCategoriaValida(contaUsuario.Conta);
            var valor = 100m;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(  tipoInvalido, contaUsuario, categoria, valor, titulo, observacao, dthrReg, null);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .WithMessage(MensagemMovimentacao.TIPO_MOV_INVALIDO);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("  ")]
        [InlineData("Ti")] // Menos de 3 caracteres
        [InlineData("Este é um título muito longo que excede o limite de oitenta caracteres para o título da movimentação")] // Mais de 80 caracteres
        public void CriarMovimentacao_ComTituloInvalido_DeveLancarExcecao(string tituloInvalido)
        {
            // Arrange
            var contaUsuario = CriarContaUsuarioValida();
            var categoria = CriarCategoriaValida(contaUsuario.Conta);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuario, categoria, valor, tituloInvalido, observacao, dthrReg, null);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .Where(e => e.Message == MensagemMovimentacao.TITULO_OBRIGATORIO || e.Message == MensagemMovimentacao.TITULO_LIMITE_CARACTERES);
        }

        [Theory]
        [InlineData("Este é um texto de observação muito longo que excede o limite de duzentos e cinquenta e cinco caracteres para a observação da movimentação, o que deve gerar uma exceção de validação. Este é um texto de observação muito longo que excede o limite de duzentos e cinquenta e cinco caracteres para a observação da movimentação, o que deve gerar uma exceção de validação. Este é um texto de observação muito longo que excede o limite de duzentos e cinquenta e cinco caracteres para a observação da movimentação, o que deve gerar uma exceção de validação.")] // Mais de 255 caracteres
        public void CriarMovimentacao_ComObservacaoInvalida_DeveLancarExcecao(string observacaoInvalida)
        {
            // Arrange
            var contaUsuario = CriarContaUsuarioValida();
            var categoria = CriarCategoriaValida(contaUsuario.Conta);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Titulo Valido";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuario, categoria, valor, titulo, observacaoInvalida, dthrReg, null);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .WithMessage(MensagemMovimentacao.OBSERVACAO_LIMITE_CARACTERES);
        }

        [Fact]
        public void CriarMovimentacao_ComContaUsuarioNula_DeveLancarExcecao()
        {
            // Arrange
            ContaUsuario contaUsuarioInvalida = null;
            var conta = CriarContaValida();
            var categoria = CriarCategoriaValida(conta);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuarioInvalida, categoria, valor, titulo, observacao, dthrReg, null);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .WithMessage(MensagemMovimentacao.USUARIO_NAO_PERTENCE_A_CONTA);
        }

        [Fact]
        public void CriarMovimentacao_ComContaUsuarioInativo_DeveLancarExcecao()
        {
            // Arrange
            var contaUsuarioInativa = CriarContaUsuarioValida(TipoStatusContasUsuario.Inativo);
            var categoria = CriarCategoriaValida(contaUsuarioInativa.Conta);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuarioInativa, categoria, valor, titulo, observacao, dthrReg, null);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .WithMessage(MensagemMovimentacao.USUARIO_INATIVO);
        }

        [Fact]
        public void CriarMovimentacao_ComContaUsuarioExpirado_DeveLancarExcecao()
        {
            // Arrange
            var conta = CriarContaValida();
            var usuarioMestre = CriarUsuario(NovoIdUsuario());
            var contaUsuarioMestre = CriarContaUsuario(conta,usuarioMestre, TiposAcessos.Mestre, TipoStatusContasUsuario.Ativo);
            var categoria = CriarCategoriaValida(contaUsuarioMestre.Conta);

            var usuario = CriarUsuario(NovoIdUsuario());
            var contaUsuario = CriarContaUsuario(conta, usuarioMestre, TiposAcessos.Administrador, TipoStatusContasUsuario.Ativo);
            contaUsuario.AtualizaOutraContaUsuario(contaUsuarioMestre,null,null, expirado: true);

            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuario, categoria, valor, titulo, observacao, dthrReg, null);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .WithMessage(MensagemMovimentacao.USUARIO_EXPIRADO);
        }

        [Fact]
        public void CriarMovimentacao_ComContaUsuarioSemPermissao_DeveLancarExcecao()
        {
            // Arrange
            var contaUsuarioSemPermissao = CriarContaUsuarioValida(acesso: TiposAcessos.Visualizador);
            var categoria = CriarCategoriaValida(contaUsuarioSemPermissao.Conta);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuarioSemPermissao, categoria, valor, titulo, observacao, dthrReg, null);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .WithMessage(MensagemMovimentacao.USUARIO_SEM_PERMISSAO);
        }

        [Fact]
        public void CriarMovimentacao_ComContaNula_DeveLancarExcecao()
        {
            // Arrange
            var contaUsuario = CriarContaUsuarioValida(criaConta:false);
            var contaParaCategoria = CriarContaValida();
            var categoria = CriarCategoriaValida(contaParaCategoria);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuario, categoria, valor, titulo, observacao, dthrReg);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
            .WithMessage(MensagemMovimentacao.CONTA_NAO_ENCONTRADA);
        }

        [Fact]
        public void CriarMovimentacao_ComCategoriaNaoPertencenteAConta_DeveLancarExcecao()
        {
            // Arrange
            var contaUsuario = CriarContaUsuarioValida();
            
            var outraConta = CriarContaValida(id: 2);
            var categoriaDeOutraConta = CriarCategoriaValida(outraConta);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuario, categoriaDeOutraConta, valor, titulo, observacao, dthrReg);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .WithMessage(MensagemMovimentacao.CATEGORIA_NAO_PERTENCA_A_CONTA);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CriarMovimentacao_ComValorInvalido_DeveLancarExcecao(decimal valorInvalido)
        {
            // Arrange
            var contaUsuario = CriarContaUsuarioValida();
            var categoria = CriarCategoriaValida(contaUsuario.Conta);
            var tipo = TipoMovimentacao.Entrada;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            var dthrReg = DateTime.UtcNow;

            // Act
            Action act = () => new Movimentacao(1, tipo, contaUsuario, categoria, valorInvalido, titulo, observacao, dthrReg);

            // Assert
            act.Should().Throw<MovimentacaoValidacao>()
               .WithMessage(MensagemMovimentacao.VALOR_DEVE_SER_MAIOR_QUE_ZERO);
        }

        [Fact]
        public void CriarMovimentacao_ComDthrRegNula_DeveAtribuirDataAtual()
        {
            // Arrange
            var contaUsuario = CriarContaUsuarioValida();
            var categoria = CriarCategoriaValida(contaUsuario.Conta);
            var tipo = TipoMovimentacao.Entrada;
            var valor = 100m;
            var titulo = "Titulo Valido";
            var observacao = "Observacao Valida";
            DateTime? dthrReg = null;

            // Act
            var movimentacao = new Movimentacao(1, tipo, contaUsuario, categoria, valor, titulo, observacao, dthrReg);

            // Assert
            movimentacao.DthrReg.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}
