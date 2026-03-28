using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Financ.Application.Mapeamento
{
    public static class ContaMapper
    {
        public static RetornaContasDTO ParaDTO(Conta dto) => new RetornaContasDTO(dto.Id,dto.Titulo,dto.Status);
    }
}
