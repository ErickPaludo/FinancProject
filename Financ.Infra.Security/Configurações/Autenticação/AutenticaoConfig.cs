using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Security.Configurações.Autenticação
{
    public class AutenticaoConfig
    {
        public string SecretKeyJWT { get; set; }
        public int ExpiracaoEmMinutos { get; set; }
        public int ExpitacaoRefreshTokenDias { get; set; }
    }
}
