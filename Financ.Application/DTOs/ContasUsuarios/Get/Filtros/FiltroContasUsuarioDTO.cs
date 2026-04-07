using Financ.Domain.Enums.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Get.Filtros
{
    public class FiltroContasUsuarioDTO
    {
        public int? Id { get; set; }
        public string? Titulo { get; set; }
        public TiposStatusContas? Status { get; set; }
    }
}
