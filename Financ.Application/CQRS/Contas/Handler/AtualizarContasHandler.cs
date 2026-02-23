using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.DTOs.Contas.Get;
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
namespace Financ.Application.CQRS.Handler
{
    public class AtualizarContasHandler : IRequestHandler<AtualizarContaCommand, Resultado<RetornaContasDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AtualizarContasHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<RetornaContasDTO>> Handle(AtualizarContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _unitOfWork.contasRepositorio.BuscarObjetoUnico(x => x.Id == request.IdConta) == null)
                    return Resultado<RetornaContasDTO>.GeraFalha(Falha.NaoEncontrado("Conta ou usuário inválidos."));

                var contaUsuario = await _unitOfWork.contasUsuariosRepositorio.BuscarObjetoUnico(x => x.IdConta == request.IdConta && x.IdUsuario == request.IdUsuario);

                if (contaUsuario == null)
                    return Resultado<RetornaContasDTO>.GeraFalha(Falha.ErroOperacional("O Usuário não pertence a está conta!"));

                if (!contaUsuario.Status.Equals(TiposStatusContas.Ativo))
                    return Resultado<RetornaContasDTO>.GeraFalha(Falha.ErroOperacional($"Não foi possível concluir a operação pois seu usuário está {contaUsuario.Status.ToString()}!"));

                var conta = await _unitOfWork.contasRepositorio.BuscarPeloId<int>(contaUsuario.IdConta);
                if (conta == null)
                    return Resultado<RetornaContasDTO>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));

                conta.AtualizaConta(contaUsuario, request.Titulo,request.Status);

                _unitOfWork.contasRepositorio.Atualiza(conta);
                await _unitOfWork.Commit();
                return Resultado<RetornaContasDTO>.GeraSucesso(ContaMapper.ParaDTO(conta));
            }
            catch (ContasValidacao contasExecao)
            {
                return Resultado<RetornaContasDTO>.GeraFalha(Falha.ErroOperacional(contasExecao.Message));
            }
        }
    }
}
