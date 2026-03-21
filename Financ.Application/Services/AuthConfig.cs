using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services
{
    public class AuthConfig
    {
        public static string PalavraChaveToken { get; set; }
        public static string Papper { get; set; } 
        public static int ExpiracaoEmMinutos { get; set; } 
        public static int ExpitacaoRefreshTokenDias { get; set; }
    }
}
