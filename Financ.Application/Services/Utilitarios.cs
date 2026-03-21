using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services
{
    public static class Utilitarios
    {
        public static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
        public static string Concatena(List<object> lista, char valorConcatenacao)
        {
            string palavra = string.Empty;
            foreach (object o in lista)
            {
                palavra += $"{o.ToString()}{valorConcatenacao}";
            }
            return palavra.TrimEnd(valorConcatenacao);
        }
        public static long DateTimeInUnixTimestamp(DateTime date)
        {
           return new DateTimeOffset(date).ToUnixTimeSeconds();
        }
        public static DateTime UnixTimestampInDateTime(long unixTimestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
        }
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
