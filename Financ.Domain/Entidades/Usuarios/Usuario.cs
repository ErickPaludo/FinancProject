using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Usuarios;
using Financ.Domain.Validacoes.Usuarios.Mensagens;
using System.Drawing;

namespace Financ.Domain.Entidades.Usuarios
{
    public sealed class Usuario : EntidadeBase
    {
        public Nome Nome { get; init; }
        public Email Endereco { get; private set; }
        public Senha Senha { get; private set; }
      
        private Usuario(Nome nome, Email endereco, Senha senha)
        {
            Nome = nome;
            Endereco = endereco;
            Senha = senha;
        }
 
        public static Usuario Create(Nome nome, Email endereco, Senha senha)
        {
            UsuariosValidacao.Verifica(nome is null, MensagensUsuarios.NOME_NULO);
            UsuariosValidacao.Verifica(endereco is null, MensagensUsuarios.EMAIL_NULO);
            UsuariosValidacao.Verifica(senha is null, MensagensUsuarios.SENHA_NULA);

            return new Usuario(nome!, endereco!, senha!);
        }

        public void AtualizaSenha(Senha senha)
        {
            UsuariosValidacao.Verifica(senha is null, MensagensUsuarios.SENHA_NULA);
            UsuariosValidacao.Verifica(Senha == senha, MensagensUsuarios.MESMA_SENHA);
            Senha = senha!;
        }
    }
}
