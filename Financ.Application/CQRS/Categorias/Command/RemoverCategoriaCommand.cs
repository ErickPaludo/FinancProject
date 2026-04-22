using Financ.Application.Comun.Resultado;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Categorias.Command
{
    public record RemoverCategoriaCommand(int IdCategoria, string IdUsuario) : IRequest<Resultado<string>>;
}
