using Financ.Application.DTOs.ContasUsuarios.Get;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Post
{
    public record RetornaPostCadastroDTO(bool Aceito, RetornaCadastroContasUsuariosDTO? ContaUsuario,string? Obs);
}
