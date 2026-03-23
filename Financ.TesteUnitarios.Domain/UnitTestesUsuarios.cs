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
        //[Fact(DisplayName = "Valida IdUsuario não informado")]
        //public void IdUsuario_NaoInformado_GeraExcecao()
        //{
        //    Action action = () => new Usuario(string.Empty, "Joao", "Silva", "teste@teste.com");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensBase.USUARIO_NAO_INFORMADO);
        //}

        //[Fact(DisplayName = "Valida primeiro nome vazio")]
        //public void PrimeiroNome_Vazio_GeraExcecao()
        //{
        //    Action action = () => new Usuario("", "Silva", "teste@teste.com");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.PRIMEIRO_NOME_OBRIGATORIO);
        //}

        //[Fact(DisplayName = "Valida primeiro nome menor que 3 caracteres")]
        //public void PrimeiroNome_MenorQue3_GeraExcecao()
        //{
        //    Action action = () => new Usuario("Jo", "Silva", "teste@teste.com");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.PRIMEIRO_NOME_MINIMO);
        //}

        //[Fact(DisplayName = "Valida primeiro nome maior que 100 caracteres")]
        //public void PrimeiroNome_MaiorQue100_GeraExcecao()
        //{
        //    var nomeGrande = new string('A', 101);

        //    Action action = () => new Usuario(nomeGrande, "Silva", "teste@teste.com");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.PRIMEIRO_NOME_MAXIMO);
        //}

        //[Fact(DisplayName = "Valida segundo nome vazio")]
        //public void SegundoNome_Vazio_GeraExcecao()
        //{
        //    Action action = () => new Usuario("Joao", "", "teste@teste.com");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.SEGUNDO_NOME_OBRIGATORIO);
        //}

        //[Fact(DisplayName = "Valida segundo nome menor que 3 caracteres")]
        //public void SegundoNome_MenorQue3_GeraExcecao()
        //{
        //    Action action = () => new Usuario("Joao", "Si", "teste@teste.com");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.SEGUNDO_NOME_MINIMO);
        //}

        //[Fact(DisplayName = "Valida segundo nome maior que 100 caracteres")]
        //public void SegundoNome_MaiorQue100_GeraExcecao()
        //{
        //    var nomeGrande = new string('A', 101);

        //    Action action = () => new Usuario("Joao", nomeGrande, "teste@teste.com");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.SEGUNDO_NOME_MAXIMO);
        //}

        //[Fact(DisplayName = "Valida email vazio")]
        //public void Email_Vazio_GeraExcecao()
        //{
        //    Action action = () => new Usuario("Joao", "Silva", "");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.EMAIL_OBRIGATORIO);
        //}

        //[Fact(DisplayName = "Valida email menor que 6 caracteres")]
        //public void Email_MenorQue6_GeraExcecao()
        //{
        //    Action action = () => new Usuario("Joao", "Silva", "a@a");

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.EMAIL_MINIMO);
        //}

        //[Fact(DisplayName = "Valida email maior que 256 caracteres")]
        //public void Email_MaiorQue256_GeraExcecao()
        //{
        //    var emailGrande = new string('a', 257);

        //    Action action = () => new Usuario("Joao", "Silva", emailGrande);

        //    action.Should()
        //          .Throw<UsuariosValidacoes>()
        //          .WithMessage(MensagensUsuarios.EMAIL_MAXIMO);
        //}

        //[Fact(DisplayName = "Criação válida de usuário")]
        //public void Usuario_Valido_NaoGeraExcecao()
        //{
        //    Action action = () => new Usuario(Guid.NewGuid().ToString(), "Joao", "Silva", "teste@teste.com");

        //    action.Should().NotThrow();
        //}
    }
}

