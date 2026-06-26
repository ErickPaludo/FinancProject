using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.Usuarios;
using Financ.Domain.Validacoes.Usuarios.Mensagens;

namespace Financ.Domain.Entidades.Usuarios
{
    public class Usuario
    {
        public string Id { get; private set; }
        public string Email { get; private set; }
        public string PrimeiroNome { get; private set; }
        public string SegundoNome { get; private set; }
        public string Salt { get; private set; }
        public string HashPass { get; private set; } = string.Empty;

        private Usuario() { }
        public Usuario(string idUsuario, string primeiroNome, string segundoNome, string email, string salt, string hashPass)
        {
            UsuariosValidacao.Verifica(string.IsNullOrEmpty(idUsuario), MensagensBase.USUARIO_NAO_INFORMADO);
            VerificaNome(primeiroNome, segundoNome);
            VerificaEmail(email);

            Id = idUsuario;
            Salt = salt;
            HashPass = hashPass;
        }

        public Usuario(string primeiroNome, string segundoNome, string email, string salt, string hashPass)
        {
            VerificaNome(primeiroNome, segundoNome);
            VerificaEmail(email);
            Id = Guid.NewGuid().ToString();

            UsuariosValidacao.Verifica(string.IsNullOrWhiteSpace(salt), MensagensUsuarios.MESMA_SENHA);
            UsuariosValidacao.Verifica(string.IsNullOrWhiteSpace(hashPass), MensagensUsuarios.MESMA_SENHA);

            Salt = salt;
            HashPass = hashPass;
        }

        public string NomeCompleto => $"{PrimeiroNome.Trim()} {SegundoNome.Trim()}";

        private void VerificaNome(string primeiroNome, string segundoNome)
        {
            UsuariosValidacao.Verifica(string.IsNullOrWhiteSpace(primeiroNome), MensagensUsuarios.PRIMEIRO_NOME_OBRIGATORIO);
            UsuariosValidacao.Verifica(string.IsNullOrWhiteSpace(segundoNome), MensagensUsuarios.SEGUNDO_NOME_OBRIGATORIO);

            // Verifica espaços inválidos
            UsuariosValidacao.Verifica(primeiroNome.StartsWith(" ") || primeiroNome.EndsWith(" ") || primeiroNome.Contains("  "),
                MensagensUsuarios.PRIMEIRO_NOME_INVALIDO);
            UsuariosValidacao.Verifica(segundoNome.StartsWith(" ") || segundoNome.EndsWith(" ") || segundoNome.Contains("  "),
                MensagensUsuarios.SEGUNDO_NOME_INVALIDO);

            // Verifica comprimento mínimo e máximo
            UsuariosValidacao.Verifica(primeiroNome.Length > 100, MensagensUsuarios.PRIMEIRO_NOME_MAXIMO);
            UsuariosValidacao.Verifica(primeiroNome.Length < 3, MensagensUsuarios.PRIMEIRO_NOME_MINIMO);

            UsuariosValidacao.Verifica(segundoNome.Length > 100, MensagensUsuarios.SEGUNDO_NOME_MAXIMO);
            UsuariosValidacao.Verifica(segundoNome.Length < 3, MensagensUsuarios.SEGUNDO_NOME_MINIMO);

            // Verifica caracteres inválidos (aceita letras, acentos, hífen e apóstrofo)
            UsuariosValidacao.Verifica(!primeiroNome.All(c => char.IsLetter(c) || c == '-' || c == '\'' || c == ' '),
                MensagensUsuarios.PRIMEIRO_NOME_INVALIDO);
            UsuariosValidacao.Verifica(!segundoNome.All(c => char.IsLetter(c) || c == '-' || c == '\'' || c == ' '),
                MensagensUsuarios.SEGUNDO_NOME_INVALIDO);


            // Atribuição final
            PrimeiroNome = primeiroNome;
            SegundoNome = segundoNome;
        }
        private void VerificaEmail(string email)
        {
            UsuariosValidacao.Verifica(string.IsNullOrWhiteSpace(email), MensagensUsuarios.EMAIL_OBRIGATORIO);
            UsuariosValidacao.Verifica(email.Length > 256, MensagensUsuarios.EMAIL_MAXIMO);
            UsuariosValidacao.Verifica(email.Length < 6, MensagensUsuarios.EMAIL_MINIMO);
            Email = email.Trim();
        }
        public void AtualizaSenha(string salt, string hashPass)
        {
            UsuariosValidacao.Verifica(salt == Salt, MensagensUsuarios.MESMA_SENHA);
            UsuariosValidacao.Verifica(hashPass == HashPass, MensagensUsuarios.MESMA_SENHA);

            Salt = salt;
            HashPass = hashPass;
        }
    }
}
