using Financ.Domain.Enums.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Get.Filtros
{
    public record FiltroUsuarioAssociado(string? IdUsuario = null, string? NomeUsuario = null, TiposAcessos? Acesso = null, TipoStatusContasUsuario? Status = null);
}
