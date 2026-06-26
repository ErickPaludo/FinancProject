using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Categorias.Command;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Categoria.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.Categorias;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Categorias.Handler
{
    public class AlterarCategoriaHandler : IRequestHandler<AlterarCategoriaCommand, Resultado<BasePost<CategoriaDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AlterarCategoriaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BasePost<CategoriaDTO>>> Handle(AlterarCategoriaCommand request, CancellationToken cancellationToken)
        {
           

                Categoria? categoria = await _unitOfWork.categoriaRepositorio.ObterCategoriaComConta(c => c.Id == request.IdCategoria);

                if (categoria is null)
                    return Resultado<BasePost<CategoriaDTO>>.GeraFalha(Falha.NaoEncontrado("Categoria não encontrada"));
      
                ContaUsuario? contaUsuario = categoria.Conta.ContaUsuarios.FirstOrDefault(c => c.IdUsuario == request.IdUsuario);

                categoria.Alterar(contaUsuario, request.Nome, request.Cor);

                _unitOfWork.categoriaRepositorio.Atualiza(categoria);
                await _unitOfWork.Commit();
                return Resultado<BasePost<CategoriaDTO>>.GeraSucesso(new BasePost<CategoriaDTO>(CategoriaMapper.ParaDTO(categoria)));
           
        }
    }
}
