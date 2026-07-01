using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Usuarios;
using Financ.Domain.Validacoes.Usuarios.Mensagens;

namespace Financ.Domain.Entidades.Usuarios
{
    public sealed class Usuario : EntidadeBase
    {
        public Nome Nome { get; init; }
        public Email Endereco { get; private set; }
        public Senha Senha { get; private set; }
      
        public Usuario(Nome nome, Email endereco, Senha senha)
        {
            Nome = nome;
            Endereco = endereco;
            Senha = senha;
        }
 
        public void AtualizaSenha(Senha senha)
        {
            UsuariosValidacao.Verifica(Senha == senha, MensagensUsuarios.MESMA_SENHA);
            Senha = senha;
        }
    }
}
