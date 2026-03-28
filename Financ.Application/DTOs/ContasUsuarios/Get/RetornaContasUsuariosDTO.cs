using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Get
{
    public sealed record RetornaContasUsuariosDTO(RetornaContasDTO conta, bool? Expirado, DateTime? Expiracao);

}
