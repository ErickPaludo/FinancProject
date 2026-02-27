using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
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
    public class AtualizarContaUsuarioHandler : IRequestHandler<AtualizarContaUsuarioCommand, Resultado<RetornaCadastroContasUsuariosDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AtualizarContaUsuarioHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<RetornaCadastroContasUsuariosDTO>> Handle(AtualizarContaUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var contasUsuarios = await _unitOfWork.contasUsuariosRepositorio.BuscarPorCondicao(x => x.IdConta == request.idConta);

                if (contasUsuarios.Count() == 0)
                    return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada!"));

                var contaUsuarioAlterado = contasUsuarios?.First(x => x.IdUsuario == request.idUsuarioAlterado);

                var contaUsuarioSolicitante = contasUsuarios?.First(x => x.IdUsuario == request.idUsuarioSolicitante);

                if (contaUsuarioAlterado is not null && contaUsuarioSolicitante is not null)
                {
                    if (contaUsuarioSolicitante.Acesso == TiposAcessos.Mestre)
                    {
                            contaUsuarioAlterado.AtualizaOutraContaUsuario(contaUsuarioSolicitante,request.acesso, request.status);
                            _unitOfWork.contasUsuariosRepositorio.Atualiza(contaUsuarioAlterado);
                            await _unitOfWork.Commit();
                            return Resultado<RetornaCadastroContasUsuariosDTO>.GeraSucesso(ContasUsuariosMapper.ParaDTO(contaUsuarioAlterado)); 
                    }
                    return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.NaoEncontrado("Você precisa ser um usuário mestre para alterar outros usuários desta conta!"));

                }

                return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.NaoEncontrado("Conta ou usuário não encontrados!"));

            }
            catch (ContasUsuariosValidacao ex)
            {
                return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
