using Financ.Application.DTOs.Convites.Get.MicroDto;
using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Convites.Get
{
    public record GetRetornaConvitesDTO(GetConvite convite,GetContaConvite conta,GetUsuarioConvite usuarioRemetente,GetUsuarioConvite usuarioDestinatario);
}
