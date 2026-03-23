using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Commands;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_Usuarios.Handler
{
    public class SairContaHandler : IRequestHandler<SairContaUsuarioCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SairContaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<string>> Handle(SairContaUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.idConta);

                if (conta is null)
                    return Resultado<string>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada"));

                ContasUsuarios? contaUsuario = conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario.Equals(request.idUsuario));

                conta.SairDaConta(contaUsuario);

                _unitOfWork.contasUsuariosRepositorio.Delete(contaUsuario!);
                _unitOfWork.contasRepositorio.Atualiza(conta);

                await _unitOfWork.Commit();

                return Resultado<string>.GeraSucesso("Usuário saiu da conta com sucesso.");
            }
            catch (ContasUsuariosValidacao contasUsuariosExcessao)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(contasUsuariosExcessao.Message));
            }
            catch (ContasValidacao contasExcessao)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(contasExcessao.Message));
            }
        }
    }
}
