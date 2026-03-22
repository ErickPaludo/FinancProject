using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Convites.Post
{
    public sealed class CriaConviteDTO
    {
        public int IdConta { get; set; }
        [Required]
        [EmailAddress]
        public string EmailDestinatario { get; set; }
        public TiposAcessos Acesso { get; set; }
        public int? expiracaoContaUsuario { get; set; }
    }
}
