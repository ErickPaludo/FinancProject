using Financ.Domain.Enums.ContasBancarias;
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
        public TiposAcessos? acesso { get; set; }
        public TipoStatusContasUsuario? status { get; set; }
        public int? expiracao { get; set; }
        public bool? expirado { get; set; }
    }
}
