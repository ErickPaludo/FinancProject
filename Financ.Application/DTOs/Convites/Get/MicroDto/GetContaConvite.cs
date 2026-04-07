using Financ.Domain.Enums.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Convites.Get.MicroDto
{
    public record GetContaConvite(int idConta,string titulo,TipoConta tipoConta);
}
