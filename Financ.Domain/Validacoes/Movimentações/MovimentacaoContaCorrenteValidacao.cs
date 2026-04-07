using Financ.Domain.Validacoes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Movimentações
{
    public class MovimentacaoContaCorrenteValidacao : BaseValidacao
    {
        public MovimentacaoContaCorrenteValidacao(string Erro) : base(Erro) { }
        public static void Verifica(bool condicao, string mensagem)
        {
            VerificaExcessao<MovimentacaoContaCorrenteValidacao>(condicao, mensagem);
        }
    }
}
