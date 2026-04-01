using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Financ.Application.CQRS.Contas_.Handler
{
    public class AtualizarContasHandler : IRequestHandler<AtualizarContaCommand, Resultado<BasePost<RetornaContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AtualizarContasHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<RetornaContasDTO>>> Handle(AtualizarContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Conta? conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.IdConta);

                if (conta is null)
                    return Resultado<BasePost<RetornaContasDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));

                ContasUsuarios? contaUsuario = conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);

                conta.AtualizaConta(contaUsuario, request.Titulo,request.Status);

                _unitOfWork.contasRepositorio.Atualiza(conta);
                await _unitOfWork.Commit();

                return Resultado<BasePost<RetornaContasDTO>>.GeraSucesso(ContasUsuariosMapper.ParaDTO(contaUsuario!, null));
            }
            catch (ContasValidacao contasExecao)
            {
                return Resultado<BasePost<RetornaContasDTO>>.GeraFalha(Falha.ErroOperacional(contasExecao.Message));
            }
        }
    }
}
