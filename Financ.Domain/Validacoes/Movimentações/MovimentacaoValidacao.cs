using Financ.Domain.Validacoes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Movimentações
{
    public class MovimentacaoValidacao : BaseValidacao
    {
        public MovimentacaoValidacao(string Erro) : base(Erro) { }
        public static void Verifica(bool condicao, string mensagem)
        {
            VerificaExcessao<MovimentacaoValidacao>(condicao, mensagem);
        }
    }
}
