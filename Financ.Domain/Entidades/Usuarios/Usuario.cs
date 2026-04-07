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
            UsuariosValidacoes.Verifica(string.IsNullOrEmpty(idUsuario), MensagensBase.USUARIO_NAO_INFORMADO);
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

            UsuariosValidacoes.Verifica(string.IsNullOrWhiteSpace(salt), MensagensUsuarios.MESMA_SENHA);
            UsuariosValidacoes.Verifica(string.IsNullOrWhiteSpace(hashPass), MensagensUsuarios.MESMA_SENHA);

            Salt = salt;
            HashPass = hashPass;
        }

        public string NomeCompleto => $"{PrimeiroNome.Trim()} {SegundoNome.Trim()}";

        private void VerificaNome(string primeiroNome, string segundoNome)
        {
            UsuariosValidacoes.Verifica(string.IsNullOrWhiteSpace(primeiroNome), MensagensUsuarios.PRIMEIRO_NOME_OBRIGATORIO);
            UsuariosValidacoes.Verifica(string.IsNullOrWhiteSpace(segundoNome), MensagensUsuarios.SEGUNDO_NOME_OBRIGATORIO);

            // Verifica comprimento mínimo e máximo
            UsuariosValidacoes.Verifica(primeiroNome.Length > 100, MensagensUsuarios.PRIMEIRO_NOME_MAXIMO);
            UsuariosValidacoes.Verifica(primeiroNome.Length < 3, MensagensUsuarios.PRIMEIRO_NOME_MINIMO);

            UsuariosValidacoes.Verifica(segundoNome.Length > 100, MensagensUsuarios.SEGUNDO_NOME_MAXIMO);
            UsuariosValidacoes.Verifica(segundoNome.Length < 3, MensagensUsuarios.SEGUNDO_NOME_MINIMO);

            // Verifica caracteres inválidos (aceita letras, acentos, hífen e apóstrofo)
            UsuariosValidacoes.Verifica(!primeiroNome.All(c => char.IsLetter(c) || c == '-' || c == '\'' || c == ' '),
                MensagensUsuarios.PRIMEIRO_NOME_INVALIDO);
            UsuariosValidacoes.Verifica(!segundoNome.All(c => char.IsLetter(c) || c == '-' || c == '\'' || c == ' '),
                MensagensUsuarios.SEGUNDO_NOME_INVALIDO);

            // Verifica espaços inválidos
            UsuariosValidacoes.Verifica(primeiroNome.StartsWith(" ") || primeiroNome.EndsWith(" ") || primeiroNome.Contains("  "),
                MensagensUsuarios.PRIMEIRO_NOME_INVALIDO);
            UsuariosValidacoes.Verifica(segundoNome.StartsWith(" ") || segundoNome.EndsWith(" ") || segundoNome.Contains("  "),
                MensagensUsuarios.SEGUNDO_NOME_INVALIDO);

            // Atribuição final
            PrimeiroNome = primeiroNome;
            SegundoNome = segundoNome;
        }
        private void VerificaEmail(string email)
        {
            UsuariosValidacoes.Verifica(string.IsNullOrWhiteSpace(email), MensagensUsuarios.EMAIL_OBRIGATORIO);
            UsuariosValidacoes.Verifica(email.Length > 256, MensagensUsuarios.EMAIL_MAXIMO);
            UsuariosValidacoes.Verifica(email.Length < 6, MensagensUsuarios.EMAIL_MINIMO);
            Email = email.Trim();
        }
        public void AtualizaSenha(string salt, string hashPass)
        {
            UsuariosValidacoes.Verifica(salt == Salt, MensagensUsuarios.MESMA_SENHA);
            UsuariosValidacoes.Verifica(hashPass == HashPass, MensagensUsuarios.MESMA_SENHA);

            Salt = salt;
            HashPass = hashPass;
        }
    }
}
