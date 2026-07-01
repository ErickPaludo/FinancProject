using Financ.Application.Comun.Enums;
using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Interfaces;
using Financ.Application.Mapeamento;
using Financ.Application.Services;
using Financ.Application.Services.PermissoesUsuarios;
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
        private readonly IValidaPermissao _validaPermissao;
        private readonly IExisteContaUsuario _encontraContaUsuario;

        public AlterarMovimentacaoHandler(IUnitOfWork unitOfWork, IValidaPermissao validaPermissao, IExisteContaUsuario encontraContaUsuario)
        {
            _unitOfWork = unitOfWork;
            _validaPermissao = validaPermissao;
            _encontraContaUsuario = encontraContaUsuario;
        }

        public async Task<Resultado<BasePost<MovimentacaoDTO>>> Handle(AlterarMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            var movimentacao = await _unitOfWork.movimentacaoRepositorio.BuscaMovimentacaoUnicaComContasUsuarios(m => m.Id == request.idMovimentacao);

            if (movimentacao is null)
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Movimentação não encontrada"));

            ContaUsuario? contaUsuario = await _encontraContaUsuario.Buscar(cu => cu.IdConta == movimentacao.IdConta && cu.IdUsuario == request.idUsuario);
            _validaPermissao.Valiidar(contaUsuario, PermissoesContasUsuarios.EditarMovimentacao);

            movimentacao.AlterarMovimentacao(contaUsuario, request.valor, request.tipo, request.titulo, request.observacao, request.dthrMovimentacao, request.dthrConclusao);

            _unitOfWork.movimentacaoRepositorio.Atualiza(movimentacao);
            await _unitOfWork.Commit();

            return Resultado<BasePost<MovimentacaoDTO>>.GeraSucesso(new BasePost<MovimentacaoDTO>(MovimentacaoMapper.ParaDTO(movimentacao)));
        }
    }
}
