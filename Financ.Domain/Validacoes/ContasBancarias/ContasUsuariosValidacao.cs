using Financ.Domain.Validacoes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.ContasBancarias
{
    public sealed class ContasUsuariosValidacao : BaseValidacao
    {
        public ContasUsuariosValidacao(string erro) : base(erro) { }
        public static void Verifica(bool condicao, string mensagem)
        {
           VerificaExcessao<ContasUsuariosValidacao>(condicao, mensagem);
        }
    }
}
