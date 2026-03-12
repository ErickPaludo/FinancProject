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
                var conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.idConta);

                if (conta is null)
                    return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada!"));

                var contaUsuarioDestinatario = conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.idUsuarioAlterado);
                if (contaUsuarioDestinatario is null)
                    return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado."));

                var contaUsuarioRemetente = conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.idUsuarioSolicitante);
                if (contaUsuarioRemetente is null)
                    return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.NaoEncontrado("Você não está cadastrado nessa conta."));

                contaUsuarioDestinatario.AtualizaOutraContaUsuario(contaUsuarioRemetente, request.acesso, request.status);
                _unitOfWork.contasUsuariosRepositorio.Atualiza(contaUsuarioDestinatario);
                await _unitOfWork.Commit();

                return Resultado<RetornaCadastroContasUsuariosDTO>.GeraSucesso(ContasUsuariosMapper.ParaDTO(contaUsuarioDestinatario));

            }
            catch (ContasUsuariosValidacao ex)
            {
                return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
