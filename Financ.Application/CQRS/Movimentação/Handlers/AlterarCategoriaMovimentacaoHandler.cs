using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Handlers
{
    public class AlterarCategoriaMovimentacaoHandler : IRequestHandler<AlterarCategoriaMovimentacaoCommand, Resultado<BasePost<MovimentacaoDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AlterarCategoriaMovimentacaoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<MovimentacaoDTO>>> Handle(AlterarCategoriaMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            Movimentacao? movimentacao = await _unitOfWork.movimentacaoRepositorio.BuscaMovimentacaoUnicaComContasUsuarios(m => m.Id == request.IdMovimentacao);

            if (movimentacao is null)
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Movimentação não encontrada"));

            ContaUsuario? contaUsuario = movimentacao.Conta.ContaUsuarios.FirstOrDefault(cu => cu.IdUsuario == request.IdUsuario);
            movimentacao.AlteraCategoriaMovimentacao(contaUsuario);

            movimentacao.CategoriasMovimentacao.ToList().ForEach(mc => _unitOfWork.movimentacaoCategoriaRepositorio.Delete(mc));

            foreach (var idCategoria in request.IdsCategoria)
            {
                Categoria? categoria = await _unitOfWork.categoriaRepositorio.BuscarObjetoUnico(c => c.Id == idCategoria && c.IdConta == movimentacao.IdConta);
                if (categoria is null)
                    return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado($"Categoria com id {idCategoria} não encontrada"));

                MovimentacaoCategoria movimentacaoCategoria = new MovimentacaoCategoria(movimentacao, categoria);
                movimentacao.AdicionarCategoria(movimentacaoCategoria);
                await _unitOfWork.movimentacaoCategoriaRepositorio.Adicionar(movimentacaoCategoria);
            }

            await _unitOfWork.Commit();

            return Resultado<BasePost<MovimentacaoDTO>>.GeraSucesso(new BasePost<MovimentacaoDTO>(MovimentacaoMapper.ParaDTO(movimentacao)));
        }
    }
}
