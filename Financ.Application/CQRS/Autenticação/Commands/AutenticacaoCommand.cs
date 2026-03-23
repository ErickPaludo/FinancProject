using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Autenticação.Get;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Autenticação.Commands
{
    public class AutenticacaoCommand : IRequest<Resultado<RetornaTokenDTO>>
    {
        public string Email { get; set; }
        public string Senha { get; set; }

        public AutenticacaoCommand(string email, string senha)
        {
            Email = email;
            Senha = senha;
        }
    }
}
