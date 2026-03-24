using Financ.Application.Interfaces;
using Financ.Application.Interfaces.Segurança;
using Financ.Infra.Security.Configurações.Segurança;
using Financ.Infra.Security.Uteis.Segurança;
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
    public class SegurancaServico : ISegurancaServico
    {
        private readonly SegurancaConfig _segurancaConfig;
        public SegurancaServico(IOptions<SegurancaConfig> segurancaConfig)
        {
            _segurancaConfig = segurancaConfig.Value;
        }
        public (string salt, string hash) CriaPassArgon(string senha,string? salt = null)
        {
            byte[] senhaBytes = Encoding.UTF8.GetBytes(senha);
            byte[] saltBytes = salt is null ? UtilSeguranca.GeraBytesAleatorios(32) : Convert.FromBase64String(salt);

            var cripto = new Argon2id(senhaBytes)
            {
                DegreeOfParallelism = 4,
                MemorySize = 8192,
                Iterations = 80,
                Salt = saltBytes,
                KnownSecret = UtilSeguranca.ConverteParaBytes(_segurancaConfig.Pepper)
            };

            var hash = cripto.GetBytes(32);
            string hashBase = Convert.ToBase64String(hash);
            string saltBase = Convert.ToBase64String(saltBytes);

            return (saltBase, hashBase);
        }

        public bool ValidaPassArgon(string senhaBanco, string senha,string salt)
        {
           return CryptographicOperations.FixedTimeEquals(
               UtilSeguranca.ConverteParaBytes(senhaBanco),
               UtilSeguranca.ConverteParaBytes(CriaPassArgon(senha, salt).hash)
            );
        } 
    }
}
