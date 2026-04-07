using Financ.Domain.Enums.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Get
{
    public record RetornaCadastroContasUsuariosDTO(int IdConta,TiposAcessos Acesso, string IdUsuario){}
}
