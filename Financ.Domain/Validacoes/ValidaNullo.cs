using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Financ.Domain.Validacoes
{
    public static class ValidaNullo
    {
        public static void Verifica([NotNull]object? objeto, string mensagem)
        {
            if (objeto == null)
                throw new ExceptionDomain(mensagem);
        }
    }
}
