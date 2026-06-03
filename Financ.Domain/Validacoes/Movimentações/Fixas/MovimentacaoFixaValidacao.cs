using Financ.Domain.Validacoes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Movimentações.Fixas
{
    public class MovimentacaoFixaValidacao : BaseValidacao
    {
        public MovimentacaoFixaValidacao(string erro) : base(erro){}
        public static void Verifica(bool condicao, string mensagem)
        {
            VerificaExcessao<MovimentacaoFixaValidacao>(condicao, mensagem);
        }
    }
}
