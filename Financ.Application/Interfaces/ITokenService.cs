using Financ.Application.DTOs.Autenticação.Get;
using Financ.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Interfaces
{
    public interface ITokenService
    {
        RetornaTokenDTO GeraToken(string idUsuario, string email);

        string GeraRefreshToken();

        void ValidaToken(string token);

        RetornaTokenDTO RefreshToken(Autenticacao autenticacao, string antigoRefreshToken);
    }
}
