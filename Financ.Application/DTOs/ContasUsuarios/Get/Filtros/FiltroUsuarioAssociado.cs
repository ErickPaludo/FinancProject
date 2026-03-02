using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Get.Filtros
{
    public class FiltroUsuarioAssociado
    {
        public int IdConta { get; set; }
        public string? IdUsuario { get; set; }
        public string? NomeUsuario { get; set; }
        public TiposAcessos? Acesso { get; set; }
        public TipoStatusContasUsuario? Status { get; set; }
    }
}
