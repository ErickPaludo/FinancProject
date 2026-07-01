using Financ.Application.Comun.Enums;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Interfaces
{
    public interface IValidaPermissao
    {
        void Valiidar(ContaUsuario usuario, PermissoesContasUsuarios acao);
    }
}
