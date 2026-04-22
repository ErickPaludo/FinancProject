using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Categorias.Command;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Categoria.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.Movimentações;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Categorias.Handler
{
    public class RemoverCategoriaHandler : IRequestHandler<RemoverCategoriaCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemoverCategoriaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<string>> Handle(RemoverCategoriaCommand request, CancellationToken cancellationToken)
        {
            try
            {

                Categoria? categoria = await _unitOfWork.categoriaRepositorio.ObterCategoriaComConta(c => c.Id == request.IdCategoria);

                if (categoria is null)
                    return Resultado<string>.GeraFalha(Falha.NaoEncontrado("Categoria não encontrada"));

                ContaUsuario? contaUsuario = categoria.Conta.ContaUsuarios.FirstOrDefault(c => c.IdUsuario == request.IdUsuario);

                categoria.Remover(contaUsuario);

                _unitOfWork.categoriaRepositorio.Delete(categoria);
                await _unitOfWork.Commit();
                return Resultado<string>.GeraSucesso("Categoria removida com sucesso");
            }
            catch (CategoriaValidacao ex)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
