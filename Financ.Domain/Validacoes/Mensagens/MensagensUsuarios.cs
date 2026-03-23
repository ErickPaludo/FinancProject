using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Mensagens
{
    public static class MensagensUsuarios
    {

        public const string PRIMEIRO_NOME_OBRIGATORIO = "O primeiro nome do usuário é obrigatório.";
        public const string PRIMEIRO_NOME_MINIMO = "O primeiro nome não deve possuir menos do que 3 caracteres";
        public const string PRIMEIRO_NOME_MAXIMO = "O primeiro nome não deve possuir mais do que 100 caracteres";

        public const string SEGUNDO_NOME_OBRIGATORIO = "O Segundo nome do usuário é obrigatório.";
        public const string SEGUNDO_NOME_MINIMO = "O segundo nome não deve possuir menos do que 3 caracteres";
        public const string SEGUNDO_NOME_MAXIMO = "O segundo nome não deve possuir mais do que 100 caracteres";

        public const string EMAIL_OBRIGATORIO = "O email do usuário é obrigatório.";
        public const string EMAIL_MINIMO = "O email não deve possuir menos do que 6 caracteres";
        public const string EMAIL_MAXIMO = "O email não deve possuir mais do que 256 caracteres";

        public const string MESMA_SENHA = "Senhas identicas.";
        public const string SENHA_VAZIA = "Obrigatório informar uma senha.";
    }
}
