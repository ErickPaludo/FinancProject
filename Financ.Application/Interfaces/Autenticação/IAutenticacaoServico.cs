using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Modelos.Autenticação;
using Financ.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Interfaces.Autenticação
{
    public interface IAutenticacaoServico
    {
        ResultadoToken GeraToken(string idUsuario, string email);

        string GeraRefreshToken();

        void ValidaToken(string token);

        ResultadoToken RefreshToken(Autenticacao autenticacao, string antigoRefreshToken);
    }
}
