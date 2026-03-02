using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Patch
{
    public class AtualizaContasUsuariosDTO
    {
        public TiposAcessos? Acesso { get; set; }
        public TipoStatusContasUsuario? Status { get; set; }
    }
}
