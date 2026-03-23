using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades
{
    public class Autenticacao
    {
        [Key]
        public string IdSession { get; private set; }
        public string IdUsuario { get; private set; }
        public string? RefreshToken { get; private set; }
        public long? ExpirationRefresh { get; private set; }
        public bool Revoke { get; private set; } = false;

        public Usuario Usuario { get; private set; }

        public Autenticacao(){}

        public Autenticacao(string idUsuario, string refreshToken, long expirationRefresh)
        {
            IdUsuario = idUsuario;
            RefreshToken = refreshToken;
            ExpirationRefresh = expirationRefresh;
        }

        public void AtualizaRefreshToken(string refreshToken, long expirationRefresh)
        {
            RefreshToken = refreshToken;
            ExpirationRefresh = expirationRefresh;
            Revoke = false;
        }
        public void RevokaToken()
        {
            Revoke = true;
        }
    }
}
