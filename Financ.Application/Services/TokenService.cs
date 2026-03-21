using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Interfaces;
using Financ.Domain.Entidades;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfig _tokenConfig;
        public TokenService(IOptions<TokenConfig> tokenConfig)
        {
            _tokenConfig = tokenConfig.Value;
        }
        private DateTime GeraExpiracao() => DateTime.UtcNow.AddMinutes(_tokenConfig.ExpiracaoEmMinutos);

        public RetornaTokenDTO GeraToken(string idUsuario, string email)
        {
            DateTime expiration = GeraExpiracao();

            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_tokenConfig.SecretKeyJWT);

            var refreshToken = GeraRefreshToken();

            var token = new JwtSecurityToken(
                claims: new[]
                {
                 new Claim(JwtRegisteredClaimNames.Sub, idUsuario),
                 new Claim(JwtRegisteredClaimNames.Email, email),
                 new Claim("sid",refreshToken)
                },
                expires: expiration,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            var tokenString = handler.WriteToken(token);

            return new RetornaTokenDTO
            {
                Token = tokenString,
                Expiracao = expiration,
                RefreshToken = refreshToken,
                ExpiracaoRefresh = DateTime.UtcNow.AddDays(_tokenConfig.ExpitacaoRefreshTokenDias),
            }; 

            byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                alg = "HS256",
                typ = "JWT"
            }));
            byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                sub = idUsuario,
                email = email,
                exp = Utilitarios.DateTimeInUnixTimestamp(expiration),
            }));

            string data = Utilitarios.Concatena(
                new List<object> {
                    { Utilitarios.Base64UrlEncode(headerBytes) },
                    { Utilitarios.Base64UrlEncode(payloadBytes) }
                }, '.');

            byte[] databytes = Encoding.UTF8.GetBytes(data);

            var assinatura = CriaAssinatura(databytes);

            //string token = Utilitarios.Concatena(
            //    new List<object> {
            //    { Utilitarios.Base64UrlEncode(headerBytes) },
            //    { Utilitarios.Base64UrlEncode(payloadBytes) },
            //    { assinatura } },
            //    '.');

            RetornaTokenDTO tokenDTO = new RetornaTokenDTO
            {
                Token = "",//token,
                Expiracao = expiration,
                RefreshToken = GeraRefreshToken(),
                ExpiracaoRefresh = DateTime.UtcNow.AddDays(_tokenConfig.ExpitacaoRefreshTokenDias),
            };

            return tokenDTO;
        }
        public string GeraRefreshToken()
        {
            string refresh = Utilitarios.GeraBase64Aleatorios(32);

         //   _tokenConfig.RefreshToken = refresh;
         //   _tokenConfig.RxpirationRefresh = Utilitarios.DateTimeInUnixTimestamp(DateTime.UtcNow.AddDays(_tokenConfig.ExpitacaoRefreshTokenDias));

            return refresh;
        }
        private string CriaAssinatura(byte[] data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_tokenConfig.SecretKeyJWT));
            byte[] hash = hmac.ComputeHash(data);
            return Utilitarios.Base64UrlEncode(hash);
        }
        public void ValidaToken(string token)
        {
            if (token.Split('.').Count() != 3)
                throw new Exception("Token inválido.");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var jwtExpiration = jwt.Claims.First(x => x.Type.Equals("exp"));
            var assignature = jwt.RawSignature;

            if (jwtExpiration is null)
                throw new Exception("Token sem data de expiração.");

            if (Utilitarios.UnixTimestampInDateTime(Convert.ToInt64(jwtExpiration.Value)) < DateTime.UtcNow)
                throw new Exception("Token expirado.");

            string headerPayload = $"{jwt.RawHeader}.{jwt.RawPayload}";

            if (assignature != CriaAssinatura(Encoding.UTF8.GetBytes(headerPayload)))
                throw new Exception("Token não autorizado.");
        }
        private void ValidaRefresh(string antigoRefreshToken, string novoRefreshToken,long expirationRefresh)
        {
            if (!antigoRefreshToken.Equals(novoRefreshToken))
                throw new Exception("Refresh Token inválido.");

            if (Utilitarios.UnixTimestampInDateTime(expirationRefresh) < DateTime.UtcNow)
                throw new Exception("Refresh Token expirado.");
        }
        public RetornaTokenDTO RefreshToken(Usuario usuario ,string antigoRefreshToken)
        {
            ValidaRefresh(antigoRefreshToken, usuario!.RefreshToken!,usuario.ExpirationRefresh!.Value);

            return GeraToken(usuario.Id, usuario.Email);
        }
    }
}
