using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Categorias.Command;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Categoria.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.Categorias;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Categorias.Handler
{
    public class CriaCategoriaHandler : IRequestHandler<CriaCategoriaCommand, Resultado<BasePost<CategoriaDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CriaCategoriaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        async Task<Resultado<BasePost<CategoriaDTO>>> IRequestHandler<CriaCategoriaCommand, Resultado<BasePost<CategoriaDTO>>>.Handle(CriaCategoriaCommand request, CancellationToken cancellationToken)
        {
           
                ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuarioPredicado(x => x.IdConta == request.IdConta && x.IdUsuario == request.IdUsuario);

                if (await _unitOfWork.categoriaRepositorio.BuscarObjetoUnico(c => c.IdConta == request.IdConta && c.Nome.Equals(request.Nome.Trim())) is not null)
                    return Resultado<BasePost<CategoriaDTO>>.GeraFalha(Falha.ErroOperacional("Já existe uma categoria cadastrada com este nome."));


                Categoria categoria = new Categoria(contaUsuario, request.Nome, request.Cor);
                await _unitOfWork.categoriaRepositorio.Adicionar(categoria);
                await _unitOfWork.Commit();
                return Resultado<BasePost<CategoriaDTO>>.GeraSucesso(new BasePost<CategoriaDTO>(CategoriaMapper.ParaDTO(categoria)));
          
        }
    }
}
