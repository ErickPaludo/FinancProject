using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Categoria.Get;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Categorias.Query
{
    public record RetornaCategoriasQuery(int IdConta,string IdUsuario) : IRequest<Resultado<BaseGetList<CategoriaDTO>>>;
}
