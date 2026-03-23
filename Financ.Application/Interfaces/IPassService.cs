using Financ.Application.Services.Segurança;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Interfaces
{
    public interface IPassService
    {
        PassArgon CriaPassArgon(string senha, string? salt = null);
        bool ValidaPassArgon(string senhaBanco, string senha, string salt);
    }
}
