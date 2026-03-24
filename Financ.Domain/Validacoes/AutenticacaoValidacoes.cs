using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes
{
    internal class AutenticacaoValidacoes : BaseValidacao
    {
        public AutenticacaoValidacoes(string erro) : base(erro) { }
        public static void Verifica(bool condicao, string mensagem)
        {
            VerificaExcessao<AutenticacaoValidacoes>(condicao, mensagem);
        }
    }
}
