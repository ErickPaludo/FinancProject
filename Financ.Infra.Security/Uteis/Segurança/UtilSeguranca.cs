using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Security.Uteis.Segurança
{
    public static class UtilSeguranca
    {
        public static byte[] ConverteParaBytes(string valor)
        {
            return Encoding.UTF8.GetBytes(valor);
        }
        public static byte[] GeraBytesAleatorios(int tamanho)
        {
            return RandomNumberGenerator.GetBytes(tamanho);
        }
        public static string GeraBase64Aleatorios(int tamanho)
        {
            return Convert.ToBase64String(GeraBytesAleatorios(tamanho));
        }
    }
}
