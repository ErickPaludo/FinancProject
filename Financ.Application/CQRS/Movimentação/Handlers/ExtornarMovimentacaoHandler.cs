using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.Movimentações;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Handlers
{
    public class ExtornarMovimentacaoHandler : IRequestHandler<ExtornarMovimentacaoCommand, Resultado<BasePost<MovimentacaoDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExtornarMovimentacaoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<MovimentacaoDTO>>> Handle(ExtornarMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var movimentacao = await _unitOfWork.movimentacaoRepositorio.BuscaMovimentacaoUnicaComContasUsuarios(m => m.Id == request.idMovimentacao);

                if (movimentacao is null)
                    return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Movimentação não encontrada"));

                ContaUsuario? usuarioExecutor = movimentacao.Conta.ContaUsuarios.FirstOrDefault(cu => cu.IdUsuario == request.idUsuario);

                movimentacao.ExtornaMovimentacao(usuarioExecutor);

                Conta conta = movimentacao.Conta;
                conta.ProcessaExtornoMovimentacao(movimentacao);

                _unitOfWork.movimentacaoRepositorio.Atualiza(movimentacao);
                _unitOfWork.contasRepositorio.Atualiza(conta);
                await _unitOfWork.Commit();

                return Resultado<BasePost<MovimentacaoDTO>>.GeraSucesso(new BasePost<MovimentacaoDTO>(MovimentacaoMapper.ParaDTO(movimentacao)));
            }
            catch (ContasValidacao ex)
            {
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
            catch (MovimentacaoValidacao ex)
            {
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
