using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Get
{
    public record RetornarContasDTO(decimal SaldoRealizado, decimal SaldoProjetado,decimal EntradaPendente,decimal SaidaPendente,List<ContasDTO> Contas);
}
