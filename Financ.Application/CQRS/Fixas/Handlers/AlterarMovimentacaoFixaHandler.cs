using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Fixas.Get;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.Movimentações.Fixas;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Handlers
{
    public class AlterarMovimentacaoFixaHandler : IRequestHandler<AlterarMovimentacaoFixaCommand, Resultado<GetMovimentacaoFixaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AlterarMovimentacaoFixaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<GetMovimentacaoFixaDTO>> Handle(AlterarMovimentacaoFixaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                MovimentacaoFixa? movimentacoaFixa = await _unitOfWork.movimentacaoFixaRepositorio.BuscaMovimentacaoFixaCompleta(x => x.Id == request.IdFixa && x.Status == StatusMovimentacaoFixa.Ativo);

                if (movimentacoaFixa is null)
                    return Resultado<GetMovimentacaoFixaDTO>.GeraFalha(Falha.NaoEncontrado("Movimentação fixa não encontrada!"));

                ContaUsuario? contaUsuario = movimentacoaFixa.Movimentacao.Conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);

                movimentacoaFixa.AlteraMovimentacaoFixa(contaUsuario,request.Tipo,request.Status,request.DataInicio,request.DataFim,request.DataOcorrencia);

                _unitOfWork.movimentacaoFixaRepositorio.Atualiza(movimentacoaFixa);
                await _unitOfWork.Commit();
                return Resultado<GetMovimentacaoFixaDTO>.GeraSucesso(MovimentacoesFixasMapper.ParaDTO(movimentacoaFixa));
            }
            catch (MovimentacaoFixaValidacao ex)
            {
                return Resultado<GetMovimentacaoFixaDTO>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
