using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Autenticação;
using Financ.Domain.Interfaces.InterfaceEntidades;
using Financ.Domain.Validacoes;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Handler
{
    public class IncluirUsuarioContaHandler : IRequestHandler<IncluiUsuarioContaCommand, Resultado<RetornaCadastroContasUsuariosDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuariosServico;
        public IncluirUsuarioContaHandler(IUnitOfWork unitOfWork,IUsuariosIdentityServicos usuariosServico)
        {
            _unitOfWork = unitOfWork;
            _usuariosServico = usuariosServico;
        }
        public async Task<Resultado<RetornaCadastroContasUsuariosDTO>> Handle(IncluiUsuarioContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!await _unitOfWork.contasRepositorio.ExisteId(x => x.Id == request.IdConta && x.Status != TiposStatus.Deletado))
                    return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.NaoEncontrado("Conta não cadastrada!"));

                if ((await _unitOfWork.contasUsuariosRepositorio.ObterContasDoUsuario(x => x.IdUsuario == request.IdUsuario && x.IdConta == request.IdConta)).Count() > 0)
                    return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.ErroOperacional("Usuário já está cadastrado nesta conta!"));

                Conta? conta = await _unitOfWork.contasRepositorio.BuscarObjetoUnico(x => x.Id == request.IdConta);
                Usuario usuario = await _usuariosServico.ObtemUsuario(request.IdUsuario);
                var contaUsuario = new ContasUsuarios(conta!, usuario.IdUsuario, request.Acesso,null);
                contaUsuario = await _unitOfWork.contasUsuariosRepositorio.Adicionar(contaUsuario);
                await _unitOfWork.Commit();
                return Resultado<RetornaCadastroContasUsuariosDTO>.GeraSucesso(ContasUsuariosMapper.ParaDTO(contaUsuario));
            }
            catch (ContasUsuariosValidacao contasUsuariosExcessao)
            {
                return Resultado<RetornaCadastroContasUsuariosDTO>.GeraFalha(Falha.ErroOperacional(contasUsuariosExcessao.Message));
            }
        }
    }
}
