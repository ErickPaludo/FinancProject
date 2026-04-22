using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Categoria.Get;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Categorias.Command
{
    public record CriaCategoriaCommand(int IdConta,string IdUsuario,string Nome, string? Cor) : IRequest<Resultado<BasePost<CategoriaDTO>>>;
}
