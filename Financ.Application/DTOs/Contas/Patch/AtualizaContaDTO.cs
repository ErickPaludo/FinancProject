using Financ.Domain.Enums.ContasBancarias;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Contas.Ptch
{
    public record AtualizaContaDTO (string? Titulo, StatusContas? Status, string? Cor);
}
