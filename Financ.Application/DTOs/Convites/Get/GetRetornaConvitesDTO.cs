using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Convites.Get
{
    public record GetRetornaConvitesDTO(int idConta,string tituloConta,string remetente,TiposAcessos acesso,DateTime expiracao);
}
