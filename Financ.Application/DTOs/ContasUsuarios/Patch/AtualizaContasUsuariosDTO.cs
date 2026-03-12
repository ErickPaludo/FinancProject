using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Patch
{
    public class AtualizaContasUsuariosDTO
    {
        [Required]
        public string? idUsuarioAlterado { get; set; }
        public TiposAcessos? Acesso { get; set; }
        public TipoStatusContasUsuario? Status { get; set; }
    }
}
