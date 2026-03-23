using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services.Segurança
{
    public class PassArgon
    {
        public string Salt { get; private set; }
        public string Hash { get; private set; }
        public PassArgon(string salt,string hash)
        {
            Salt = salt;
            Hash = hash;
        }
    }
}
