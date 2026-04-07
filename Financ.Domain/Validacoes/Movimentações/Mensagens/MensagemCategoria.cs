using Financ.Domain.Validacoes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Movimentações.Mensagens
{
    public static class MensagemCategoria
    {
        public static string NOME_OBRIGATORIO => "O nome da categoria é obrigatório.";
        public static string CARACTERES_INVALIDOS => "O nome da categoria deve ter entre 3 e 50 caracteres.";
    }
}
