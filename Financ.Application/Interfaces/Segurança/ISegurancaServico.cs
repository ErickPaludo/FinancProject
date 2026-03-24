using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Interfaces.Segurança
{
    public interface ISegurancaServico
    {
        (string salt,string hash) CriaPassArgon(string senha, string? salt = null);
        bool ValidaPassArgon(string senhaBanco, string senha, string salt);
    }
}
