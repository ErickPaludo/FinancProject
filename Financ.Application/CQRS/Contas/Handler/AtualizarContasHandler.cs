using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums;
using Financ.Domain.Enums.Movimentações;
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
    public class AtualizarContasHandler : IRequestHandler<AtualizarContaCommand, Resultado<BaseGet<ContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AtualizarContasHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BaseGet<ContasDTO>>> Handle(AtualizarContaCommand request, CancellationToken cancellationToken)
        {
                Conta? conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuariosEConvintes(x => x.Id == request.IdConta);

                if (conta is null)
                    return Resultado<BaseGet<ContasDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));

                ContaUsuario? contaUsuario = conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);

                conta.AtualizaConta(contaUsuario, request.Titulo,request.Status,request.Cor);

                _unitOfWork.contasRepositorio.Atualiza(conta);
                await _unitOfWork.Commit();

                IEnumerable<Movimentacao> movimentacoes = await _unitOfWork.movimentacaoRepositorio.BuscarPorCondicao(m => m.IdConta == request.IdConta && m.Status == StatusMovimentacao.Pendente);

                return Resultado<BaseGet<ContasDTO>>.GeraSucesso(ContaUsuarioMapper.ParaGetDTO(contaUsuario,movimentacoes));
        }
    }
}
