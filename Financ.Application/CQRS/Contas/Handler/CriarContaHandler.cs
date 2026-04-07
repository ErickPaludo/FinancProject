using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.ContasBancarias;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_.Handler
{
    public class CriarContaHandler : IRequestHandler<CriarContaCommand, Resultado<BasePost<RetornaContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CriarContaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<RetornaContasDTO>>> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Conta conta = new Conta(request.Titulo, request.Cor);
                ContaUsuario contaUsuario = new ContaUsuario(conta, request.IdUsuario);

                await _unitOfWork.contasUsuariosRepositorio.Adicionar(contaUsuario); //Cria a conta e a conta usuario pois os objetos estão linkados
                await _unitOfWork.Commit();

                return Resultado<BasePost<RetornaContasDTO>>.GeraSucesso(ContasUsuariosMapper.ParaDTO(contaUsuario,null));
            }
            catch (ContasValidacao contasExecao)
            {
                return Resultado<BasePost<RetornaContasDTO>>.GeraFalha(Falha.ErroOperacional(contasExecao.Message));
            }
            catch (ContasUsuariosValidacao contasUseuariosExcessao)
            {
                return Resultado<BasePost<RetornaContasDTO>>.GeraFalha(Falha.ErroOperacional(contasUseuariosExcessao.Message));
            }
        }
    }
}
