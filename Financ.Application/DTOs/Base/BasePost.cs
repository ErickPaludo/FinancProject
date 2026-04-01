using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Base
{
    public record BasePost<T>(T Valor) where T : class;

}
