using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Modelos.Autenticação
{
    public record ResultadoToken(string token, DateTime expirationTokenFormatado, string refreshToken,long expirationRefreshToken, DateTime expirationRefreshTokenFormatado);
}
