using Financ.Application.DTOs.Usuarios.Get;
using Financ.Domain.Entidades.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Mapeamento
{
    public static class UsuarioMapper
    {
        public static RetornaUsuarioDTO ParaDTO(Usuario usuario) => new RetornaUsuarioDTO
        {
            Id = usuario.Id,
            PrimeiroNome = usuario.PrimeiroNome,
            SegundoNome = usuario.SegundoNome,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email
        };
    }
}
