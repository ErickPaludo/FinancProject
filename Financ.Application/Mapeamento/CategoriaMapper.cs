using Financ.Application.DTOs.Categoria.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Domain.Entidades.Categorias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Mapeamento
{
    public static class CategoriaMapper
    {
        public static CategoriaDTO ParaDTO(Categoria categoria) => new CategoriaDTO(categoria.Id, categoria.Nome, categoria.Cor.Valor);
        public static List<CategoriaDTO> ParaListDTO(IEnumerable<Categoria> categoria) => categoria.Select(ParaDTO).ToList();

    }
}
