using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Autenticação;
using Financ.Domain.Validacoes;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Handler
{
    public class RemoveContaUsuarioHandler : IRequestHandler<RemoveContaUsuarioCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemoveContaUsuarioHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<string>> Handle(RemoveContaUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {

                Conta? conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id.Equals(request.idConta));

                if (conta is null)
                    return Resultado<string>.GeraFalha(Falha.NaoEncontrado("A conta informada não existe ou já foi removida."));

                ContasUsuarios? contaUsuarioRemetente = conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario.Equals(request.idUsuarioRemetente));

                if (contaUsuarioRemetente is null)
                    return Resultado<string>.GeraFalha(Falha.NaoEncontrado("O usuário remetente não pertence a conta informada."));

                ContasUsuarios? contaUsuarioDestinatario = conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario.Equals(request.idUsuarioDestinatario));

                if (contaUsuarioDestinatario is null)
                    return Resultado<string>.GeraFalha(Falha.NaoEncontrado("O usuário destinatario não pertence a conta informada."));

                contaUsuarioDestinatario.RemoverUsuarioDaConta(contaUsuarioRemetente);
                _unitOfWork.contasUsuariosRepositorio.Delete(contaUsuarioDestinatario);

                return Resultado<string>.GeraSucesso("Usuário removido da conta com sucesso!");
            }
            catch (ContasUsuariosValidacao contasUsuariosExcessao)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(contasUsuariosExcessao.Message));
            }
        }
    }
}
