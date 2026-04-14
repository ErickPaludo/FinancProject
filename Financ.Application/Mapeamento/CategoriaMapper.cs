using Financ.Application.DTOs.Categorias.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Domain.Entidades.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Mapeamento
{
    public static class CategoriaMapper
    {
        public static RetornaCategoriasDTO ParaDTO(Categoria categoria)
        {
            return new RetornaCategoriasDTO(categoria.Id, categoria.Cor.Valor);
        }
    }
}
