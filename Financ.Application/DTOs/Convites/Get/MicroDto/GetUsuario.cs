using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Convites.Get.MicroDto
{
    public record GetUsuario(string idUsuario,string email,string primeiroNome,string segundoNome,string nomeCompleto);
}
