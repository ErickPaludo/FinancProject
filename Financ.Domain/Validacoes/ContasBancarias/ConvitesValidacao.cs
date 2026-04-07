using Financ.Domain.Validacoes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.ContasBancarias
{
    public sealed class ConvitesValidacao : BaseValidacao
    {
        public ConvitesValidacao(string Erro) : base(Erro) { }
        public static void Verifica(bool condicao, string mensagem)
        {
            VerificaExcessao<ConvitesValidacao>(condicao, mensagem);
        }
    }
}
