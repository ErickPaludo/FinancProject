using Financ.Domain.Entidades;
using Financ.Domain.Interfaces.Autenticação;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Identity
{
    public class UsuariosIdentityServicos : IUsuariosIdentityServicos
    {
        private readonly UserManager<UsuarioIdentity> _gerenciaUsuarios;
        private readonly SignInManager<UsuarioIdentity> _gerenciaLogin;
        private readonly IConfiguration _configuration;
        public UsuariosIdentityServicos(UserManager<UsuarioIdentity> gerenciaUsuarios, SignInManager<UsuarioIdentity> gerenciaLogin, IConfiguration configuration)
        {
            _gerenciaUsuarios = gerenciaUsuarios;
            _gerenciaLogin = gerenciaLogin;
            _configuration = configuration;
        }
        public async Task<string> ObtemIdUsuario(string email)
        {
            var usuario = await _gerenciaUsuarios.FindByEmailAsync(email);
            if (usuario == null)
                return string.Empty;

            return _gerenciaUsuarios.GetUserIdAsync(usuario!).Result;
        }
        public async Task<(bool, string?)> RegistrarUsuario(Usuario usuario, string senha)
        {
            var usuarioidentity = new UsuarioIdentity
            {
                Email = usuario.Email,
                UserName = usuario.Email,
                PrimeiroNome = usuario.PrimeiroNome,
                SegundoNome = usuario.SegundoNome
            };
            var usuarioCriado = await _gerenciaUsuarios.CreateAsync(usuarioidentity, senha);

            return (usuarioCriado.Succeeded, string.Join("\n", usuarioCriado.Errors.Select(x => x.Description)));
        }

        public async Task<Usuario> ObtemUsuario(string idUsuario)
        {
            var usuario = _gerenciaUsuarios.FindByIdAsync(idUsuario.ToString()).Result;

            return new Usuario(idUsuario, usuario!.PrimeiroNome, usuario.SegundoNome, usuario.Email!,"","","",123);
        }
    }
}
