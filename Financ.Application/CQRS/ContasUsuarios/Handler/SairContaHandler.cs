using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Autenticação;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Handler
{
    public class SairContaHandler : IRequestHandler<SairContaUsuarioCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuarioServicos;
        public SairContaHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuarioServicos)
        {
            _unitOfWork = unitOfWork;
            _usuarioServicos = usuarioServicos;
        }
        public async Task<Resultado<string>> Handle(SairContaUsuarioCommand request, CancellationToken cancellationToken)
        {
            var contasUsuarios = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.idConta);

            if (contasUsuarios is null)
                return Resultado<string>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada"));

            ContasUsuarios contaUsuario = contasUsuarios.ContaUsuarios.FirstOrDefault(x => x.IdUsuario.Equals(request.idUsuario));

            if (contaUsuario is null)
                return Resultado<string>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado na conta."));

            contaUsuario.SairDaConta();
             _unitOfWork.contasUsuariosRepositorio.Delete(contaUsuario);
            await _unitOfWork.Commit();

            return Resultado<string>.GeraSucesso("Usuário saiu da conta com sucesso.");
        }
    }
}
