using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financ.Domain.Enums.ContasBancarias;

namespace Financ.Application.DTOs.ContasUsuarios.Get
{
    public sealed record RetornaContasDTO(int IdConta, string Titulo,bool ContaFavorita, string Cor, TiposStatusContas Status,decimal SaldoAtual,decimal SaldoProjetado, decimal EntradaPendente,decimal SaidaPendente, DateTime? Expiracao);
}
