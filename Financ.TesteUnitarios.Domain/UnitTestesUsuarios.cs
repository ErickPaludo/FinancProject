using Financ.Domain.Entidades;
using Financ.Domain.Validacoes;
using Financ.Domain.Validacoes.Mensagens;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.TesteUnitarios.Domain
{
    public class UnitTestesUsuarios
    {
        private Usuario CriarUsuarioValido(string idUsuario = null)
        {
            return new Usuario(
                idUsuario ?? Guid.NewGuid().ToString(),
                "Primeiro",
                "Sobrenome",
                "email@teste.com",
                "salt123",
                "hash123");
        }

        #region Construtores

        // Construtor: public Usuario(string idUsuario, string primeiroNome, string segundoNome, string email, string salt, string hashPass)
        [Fact(DisplayName = "Construtor com IdUsuario - Deve criar usuário válido")]
        public void ConstrutorComIdUsuario_DadosValidos_NaoDeveLancarExcecao()
        {
            Action action = () => new Usuario(
                Guid.NewGuid().ToString(),
                "Joao",
                "Silva",
                "joao.silva@teste.com",
                "salt_teste",
                "hash_teste");

            action.Should().NotThrow();
        }

        [Fact(DisplayName = "Construtor com IdUsuario - Deve atribuir propriedades corretamente")]
        public void ConstrutorComIdUsuario_DadosValidos_DeveAtribuirPropriedadesCorretamente()
        {
            var id = Guid.NewGuid().ToString();
            var primeiroNome = "Joao";
            var segundoNome = "Silva";
            var email = "joao.silva@teste.com";
            var salt = "salt_teste";
            var hashPass = "hash_teste";

            var usuario = new Usuario(id, primeiroNome, segundoNome, email, salt, hashPass);

            usuario.Id.Should().Be(id);
            usuario.PrimeiroNome.Should().Be(primeiroNome);
            usuario.SegundoNome.Should().Be(segundoNome);
            usuario.Email.Should().Be(email);
            usuario.Salt.Should().Be(salt);
            usuario.HashPass.Should().Be(hashPass);
        }

        [Fact(DisplayName = "Construtor com IdUsuario - IdUsuario não informado gera exceção")]
        public void ConstrutorComIdUsuario_IdUsuarioNaoInformado_GeraExcecao()
        {
            Action action = () => new Usuario(
                string.Empty,
                "Joao",
                "Silva",
                "teste@teste.com",
                "salt",
                "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensBase.USUARIO_NAO_INFORMADO);
        }

        // Construtor: public Usuario(string primeiroNome, string segundoNome, string email, string salt, string hashPass)
        [Fact(DisplayName = "Construtor sem IdUsuario - Deve criar usuário válido e gerar novo Id")]
        public void ConstrutorSemIdUsuario_DadosValidos_NaoDeveLancarExcecaoEGerarId()
        {
            var usuario = new Usuario("Maria", "Souza", "maria.souza@teste.com", "salt_new", "hash_new");

            usuario.Should().NotBeNull();
            usuario.Id.Should().NotBeNullOrEmpty();
            usuario.PrimeiroNome.Should().Be("Maria");
            usuario.SegundoNome.Should().Be("Souza");
            usuario.Email.Should().Be("maria.souza@teste.com");
            usuario.Salt.Should().Be("salt_new");
            usuario.HashPass.Should().Be("hash_new");
        }

        [Fact(DisplayName = "Construtor sem IdUsuario - Salt vazio gera exceção")]
        public void ConstrutorSemIdUsuario_SaltVazio_GeraExcecao()
        {
            Action action = () => new Usuario("Joao", "Silva", "teste@teste.com", string.Empty, "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.MESMA_SENHA);
        }

        [Fact(DisplayName = "Construtor sem IdUsuario - HashPass vazio gera exceção")]
        public void ConstrutorSemIdUsuario_HashPassVazio_GeraExcecao()
        {
            Action action = () => new Usuario("Joao", "Silva", "teste@teste.com", "salt", string.Empty);

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.MESMA_SENHA);
        }

        // Testes de validação de nome (comuns a ambos os construtores)
        [Fact(DisplayName = "Primeiro nome vazio gera exceção")]
        public void PrimeiroNome_Vazio_GeraExcecao()
        {
            Action action = () => new Usuario(Guid.NewGuid().ToString(), "", "Silva", "teste@teste.com", "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.PRIMEIRO_NOME_OBRIGATORIO);
        }

        [Fact(DisplayName = "Primeiro nome menor que 3 caracteres gera exceção")]
        public void PrimeiroNome_MenorQue3_GeraExcecao()
        {
            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Jo", "Silva", "teste@teste.com", "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.PRIMEIRO_NOME_MINIMO);
        }

        [Fact(DisplayName = "Primeiro nome maior que 100 caracteres gera exceção")]
        public void PrimeiroNome_MaiorQue100_GeraExcecao()
        {
            var nomeGrande = new string('A', 101);

            Action action = () => new Usuario(Guid.NewGuid().ToString(), nomeGrande, "Silva", "teste@teste.com", "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.PRIMEIRO_NOME_MAXIMO);
        }

        [Fact(DisplayName = "Segundo nome vazio gera exceção")]
        public void SegundoNome_Vazio_GeraExcecao()
        {
            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao", "", "teste@teste.com", "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.SEGUNDO_NOME_OBRIGATORIO);
        }

        [Fact(DisplayName = "Segundo nome menor que 3 caracteres gera exceção")]
        public void SegundoNome_MenorQue3_GeraExcecao()
        {
            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao", "Si", "teste@teste.com", "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.SEGUNDO_NOME_MINIMO);
        }

        [Fact(DisplayName = "Segundo nome maior que 100 caracteres gera exceção")]
        public void SegundoNome_MaiorQue100_GeraExcecao()
        {
            var nomeGrande = new string('A', 101);

            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao", nomeGrande, "teste@teste.com", "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.SEGUNDO_NOME_MAXIMO);
        }

        // Testes de validação de email (comuns a ambos os construtores)
        [Fact(DisplayName = "Email vazio gera exceção")]
        public void Email_Vazio_GeraExcecao()
        {
            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao", "Silva", "", "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.EMAIL_OBRIGATORIO);
        }

        [Fact(DisplayName = "Email menor que 6 caracteres gera exceção")]
        public void Email_MenorQue6_GeraExcecao()
        {
            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao", "Silva", "a@a", "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.EMAIL_MINIMO);
        }

        [Fact(DisplayName = "Email maior que 256 caracteres gera exceção")]
        public void Email_MaiorQue256_GeraExcecao()
        {
            var emailGrande = new string('a', 257);

            Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao", "Silva", emailGrande, "salt", "hash");

            action.Should()
                  .Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.EMAIL_MAXIMO);
        }

        #endregion

        #region NomeCompleto

        [Fact(DisplayName = "NomeCompleto - Deve retornar nome completo formatado")]
        public void NomeCompleto_DeveRetornarNomeCompletoFormatado()
        {
            var usuario = new Usuario("Primeiro", "Segundo", "email@teste.com", "salt", "hash");
            usuario.NomeCompleto.Should().Be("Primeiro Segundo");
        }

        [Fact(DisplayName = "NomeCompleto - Deve retornar nome completo com espaços extras removidos")]
        public void NomeCompleto_ComEspacosExtras_DeveRetornarNomeCompletoFormatado()
        {
            // Usando reflection para definir propriedades privadas para simular espaços extras
            var usuario = new Usuario("Primeiro", "Segundo", "email@teste.com", "salt", "hash");
            typeof(Usuario).GetProperty(nameof(Usuario.PrimeiroNome), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(usuario, "  Primeiro  ");
            typeof(Usuario).GetProperty(nameof(Usuario.SegundoNome), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(usuario, "  Segundo  ");

            usuario.NomeCompleto.Should().Be("Primeiro Segundo");
        }

        #endregion

        #region AtualizaSenha

        [Fact(DisplayName = "AtualizaSenha - Deve atualizar salt e hashPass com sucesso")]
        public void AtualizaSenha_DadosValidos_DeveAtualizarComSucesso()
        {
            var usuario = CriarUsuarioValido();
            var novoSalt = "novo_salt";
            var novoHashPass = "novo_hash";

            usuario.AtualizaSenha(novoSalt, novoHashPass);

            usuario.Salt.Should().Be(novoSalt);
            usuario.HashPass.Should().Be(novoHashPass);
        }

        [Fact(DisplayName = "AtualizaSenha - Deve lançar exceção se novo salt for igual ao atual")]
        public void AtualizaSenha_NovoSaltIgualAoAtual_DeveLancarExcecao()
        {
            var usuario = CriarUsuarioValido();
            var saltAtual = usuario.Salt;

            Action action = () => usuario.AtualizaSenha(saltAtual, "outro_hash");

            action.Should().Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.MESMA_SENHA);
        }

        [Fact(DisplayName = "AtualizaSenha - Deve lançar exceção se novo hashPass for igual ao atual")]
        public void AtualizaSenha_NovoHashPassIgualAoAtual_DeveLancarExcecao()
        {
            var usuario = CriarUsuarioValido();
            var hashPassAtual = usuario.HashPass;

            Action action = () => usuario.AtualizaSenha("outro_salt", hashPassAtual);

            action.Should().Throw<UsuariosValidacoes>()
                  .WithMessage(MensagensUsuarios.MESMA_SENHA);
        }

        #endregion
    }
}
