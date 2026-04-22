using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Categorias.Query;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Categoria.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Categorias.Handler
{
    internal class RetornaCategoriasHandler : IRequestHandler<RetornaCategoriasQuery, Resultado<BaseGetList<CategoriaDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RetornaCategoriasHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGetList<CategoriaDTO>>> Handle(RetornaCategoriasQuery request, CancellationToken cancellationToken)
        {
            var categorias = await _unitOfWork.categoriaRepositorio.BuscarPorCondicao(x => x.IdConta == request.IdConta);
            return Resultado<BaseGetList<CategoriaDTO>>.GeraSucesso(new BaseGetList<CategoriaDTO>(CategoriaMapper.ParaListDTO(categorias)));
        }
    }
}
