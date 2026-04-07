using Financ.Domain.Validacoes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Validacoes.Usuarios
{
    public sealed class UsuariosValidacoes : BaseValidacao
    {
        public UsuariosValidacoes(string Erro) : base(Erro) { }
        public static void Verifica(bool condicao, string mensagem)
        {
            VerificaExcessao<UsuariosValidacoes>(condicao, mensagem);
        }
    }
}
