using Financ.Application.Configurações;
using Financ.Application.Interfaces;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services.Segurança
{
    public class PassService : IPassService
    {
        private readonly PassConfig _passConfig;
        public PassService(IOptions<PassConfig> passConfig)
        {
            _passConfig = passConfig.Value;
        }
        public PassArgon CriaPassArgon(string senha,string? salt = null)
        {
            byte[] senhaBytes = Encoding.UTF8.GetBytes(senha);
            byte[] saltBytes = salt is null ? Utilitarios.GeraBytesAleatorios(32) : Convert.FromBase64String(salt);

            var cripto = new Argon2id(senhaBytes)
            {
                DegreeOfParallelism = 4,
                MemorySize = 8192,
                Iterations = 80,
                Salt = saltBytes,
                KnownSecret =  Utilitarios.ConverteParaBytes(_passConfig.Pepper)
            };

            var hash = cripto.GetBytes(32);
            string hashBase = Convert.ToBase64String(hash);
            string saltBase = Convert.ToBase64String(saltBytes);

            return new PassArgon(saltBase, hashBase);
        }

        public bool ValidaPassArgon(string senhaBanco, string senha,string salt)
        {
           return CryptographicOperations.FixedTimeEquals(
               Utilitarios.ConverteParaBytes(senhaBanco),
               Utilitarios.ConverteParaBytes(CriaPassArgon(senha, salt).Hash)
            );
        } 
    }
}
