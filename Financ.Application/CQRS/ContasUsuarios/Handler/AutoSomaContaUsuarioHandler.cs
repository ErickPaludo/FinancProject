using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.ContasUsuarios.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.ContasUsuarios.Handler
{
    public class AutoSomaContaUsuarioHandler : IRequestHandler<AutoSomaContaUsuarioCommand, Resultado<BaseGet<ContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AutoSomaContaUsuarioHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BaseGet<ContasDTO>>> Handle(AutoSomaContaUsuarioCommand request, CancellationToken cancellationToken)
        {
            ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuarioPredicado(c => c.IdConta == request.IdConta && c.IdUsuario == request.IdUsuario);

            if (contaUsuario is null)
                return Resultado<BaseGet<ContasDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));

            contaUsuario.AutoSomaConta();
            _unitOfWork.contasUsuariosRepositorio.Atualiza(contaUsuario);
            await _unitOfWork.Commit();
            IEnumerable<Movimentacao>? movimentacao = await _unitOfWork.movimentacaoRepositorio.BuscarPorCondicao(m => m.IdConta == request.IdConta && m.Status == StatusMovimentacao.Pendente);

            return Resultado<BaseGet<ContasDTO>>.GeraSucesso(ContaUsuarioMapper.ParaGetDTO(contaUsuario, movimentacao));
        }
    }
}
