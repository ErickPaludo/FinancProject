using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Base
{
    public record Data<T>(List<T>? Conteudo, Meta? Metadados) where T : class;
}
