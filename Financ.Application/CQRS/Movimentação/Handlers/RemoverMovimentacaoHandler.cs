using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
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
    public class RemoverMovimentacaoHandler : IRequestHandler<RemoverMovimentacaoCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemoverMovimentacaoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<string>> Handle(RemoverMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var movimentacao = await _unitOfWork.movimentacaoRepositorio.BuscaMovimentacaoUnicaComContasUsuarios(m => m.Id == request.idMovimentacao);

                if (movimentacao is null)
                    return Resultado<string>.GeraFalha(Falha.NaoEncontrado("Movimentação não encontrada"));

                ContaUsuario? usuarioExecutor = await _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuario().FirstOrDefaultAsync(cu => cu.IdConta == movimentacao.IdConta && cu.IdUsuario == request.idUsuario);


                movimentacao.ExcluiMovimentacao(usuarioExecutor);

                Conta conta = movimentacao.Conta;
                conta.RemoverMovimentacao(movimentacao);

                _unitOfWork.movimentacaoRepositorio.Delete(movimentacao);
                _unitOfWork.contasRepositorio.Atualiza(conta);
                await _unitOfWork.Commit();

                return Resultado<string>.GeraSucesso("Movimentação removida com sucesso!");
            }
            catch (ContasValidacao ex)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
            catch (MovimentacaoValidacao ex)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
