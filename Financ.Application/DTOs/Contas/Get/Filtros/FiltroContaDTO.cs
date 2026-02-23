using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Contas.Get.Filtros
{
    public class FiltroContaDTO
    {
        public int? Id { get; set; }
        public string? Titulo { get; set; }
        public TiposStatusContas? Status { get; set; }
    }
}
