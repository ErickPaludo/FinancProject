using Financ.Application.DTOs.Contas.NovaPasta;
using Financ.Application.DTOs.ContasUsuarios.Get;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Get
{
    public record DashboardTotalContasDTO(ContaDashDTO Conta, List<DashAgrupadoPorMes> Dashboard );
    public record DashAgrupadoPorMes(int Ano, int Mes, GrupoMovDashTotalDTO Movimentacao);
}
