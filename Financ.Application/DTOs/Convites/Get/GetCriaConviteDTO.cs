using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Convites.Get
{
    public record GetCriaConviteDTO(int idConvite,TiposAcessos permissao);
}
