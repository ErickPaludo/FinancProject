using Financ.Application.DTOs.Convites.Get;
using Financ.Application.DTOs.Usuarios.Get;
using Financ.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Mapeamento
{
    public static class ConvitesMapper
    {
        public static GetCriaConviteDTO ParaDTO(Convites convite) => new GetCriaConviteDTO(convite.Id, convite.Acesso);
    }
}
