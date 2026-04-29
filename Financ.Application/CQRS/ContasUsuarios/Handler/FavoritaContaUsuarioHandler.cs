using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.ContasUsuarios.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
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
    public record FavoritaContaUsuarioHandler : IRequestHandler<FavoritaContaUsuarioCommand, Resultado<BaseGet<RetornaContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoritaContaUsuarioHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGet<RetornaContasDTO>>> Handle(FavoritaContaUsuarioCommand request, CancellationToken cancellationToken)
        {
            ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuarioPredicado(c => c.IdConta == request.IdConta && c.IdUsuario == request.IdUsuario);
            
            if(contaUsuario is null)
                return Resultado<BaseGet<RetornaContasDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));

            contaUsuario.FavoritarConta();
             _unitOfWork.contasUsuariosRepositorio.Atualiza(contaUsuario);
            await _unitOfWork.Commit();
            IEnumerable<Movimentacao>? movimentacao = await _unitOfWork.movimentacaoRepositorio.BuscarPorCondicao(m => m.IdConta == request.IdConta && m.Status == TipoStatusMovimentacao.Pendente);



            return Resultado<BaseGet<RetornaContasDTO>>.GeraSucesso(ContaUsuarioMapper.ParaGetDTO(contaUsuario, movimentacao));
            throw new NotImplementedException();
        }
    }
}
