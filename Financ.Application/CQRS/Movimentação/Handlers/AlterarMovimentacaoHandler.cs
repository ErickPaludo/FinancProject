using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.Movimentações;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Handlers
{
    public class AlterarMovimentacaoHandler : IRequestHandler<AlterarMovimentacaoCommand, Resultado<BasePost<MovimentacaoDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AlterarMovimentacaoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BasePost<MovimentacaoDTO>>> Handle(AlterarMovimentacaoCommand request, CancellationToken cancellationToken)
        {
                var movimentacao = await _unitOfWork.movimentacaoRepositorio.BuscaMovimentacaoUnicaComContasUsuarios(m => m.Id == request.idMovimentacao);

                if (movimentacao is null)
                    return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Movimentação não encontrada"));

                ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuario().FirstOrDefaultAsync(cu => cu.IdConta == movimentacao.IdConta && cu.IdUsuario == request.idUsuario);


                movimentacao.AlterarMovimentacao(contaUsuario,request.valor,request.tipo,request.titulo,request.observacao,request.dthrMovimentacao,request.dthrConclusao);

                _unitOfWork.movimentacaoRepositorio.Atualiza(movimentacao);
                await _unitOfWork.Commit();

                return Resultado<BasePost<MovimentacaoDTO>>.GeraSucesso(new BasePost<MovimentacaoDTO>(MovimentacaoMapper.ParaDTO(movimentacao)));        
        }
    }
}
