using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financ.Domain.Enums;

namespace Financ.Application.DTOs.Contas.Get
{
    public sealed record RetornaContasDTO
    (
        int IdConta,
        string Titulo,
        TiposStatusContas Status
    );
}
