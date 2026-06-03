using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.DTOs.Base;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Handlers
{
    public class CriaMovimentacaoFixaDiariaHandler : IRequestHandler<CriaMovimentacaoFixaDiariaCommand, Resultado<BasePost<string>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CriaMovimentacaoFixaDiariaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<string>>> Handle(CriaMovimentacaoFixaDiariaCommand request, CancellationToken cancellationToken)
        {
            ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuarioPredicado(x => x.IdUsuario == request.idUsuario && x.IdConta == request.idConta);

            Movimentacao movimentacao = new Movimentacao(request.tipo, contaUsuario, request.valor, request.titulo, request.observacao, null, null, false);

            MovimentacaoFixa fixa = new MovimentacaoFixa(request.DataInicio, request.DataFim, request.OcorrenciasDiarias, movimentacao);
            await _unitOfWork.movimentacaoRepositorio.Adicionar(movimentacao);
            await _unitOfWork.movimentacaoFixaRepositorio.Adicionar(fixa);

            fixa.DiasFixosDiarios!.ToList().ForEach(async x => await _unitOfWork.movimentacaoFixaDiariaRepositorio.Adicionar(x));

            await _unitOfWork.Commit();
            return Resultado<BasePost<string>>.GeraSucesso(new BasePost<string>("Movimentação fixa gerada com sucesso"));
        }
    }
}

