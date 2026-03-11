using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Post
{
    public class RemoveContaUsuarioDTO
    {
        [Required]
        public int idConta { get; set; }
        [Required]
        public string idUsuarioDestinatario { get; set; } = string.Empty;
    }
}
