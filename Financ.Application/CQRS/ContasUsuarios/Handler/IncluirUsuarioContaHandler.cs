using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Post;
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
    public class IncluirUsuarioContaHandler : IRequestHandler<IncluiUsuarioContaCommand, Resultado<RetornaPostCadastro>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuariosServico;
        public IncluirUsuarioContaHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuariosServico)
        {
            _unitOfWork = unitOfWork;
            _usuariosServico = usuariosServico;
        }
        public async Task<Resultado<RetornaPostCadastro>> Handle(IncluiUsuarioContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var convite = await _unitOfWork.convitesRepostorio.BuscarObjetoUnico(x => x.Id == request.IdConvite && x.IdUsuarioDestinatario.Equals(request.IdUsuario) && x.Aceito == null);
                if (convite is null)
                    return Resultado<RetornaPostCadastro>.GeraFalha(Falha.NaoEncontrado("Convite não encontrado!"));

                if ((await _unitOfWork.contasUsuariosRepositorio.ExisteId(x => x.IdUsuario == request.IdUsuario && x.IdConta == convite.IdConta)))
                    return Resultado<RetornaPostCadastro>.GeraFalha(Falha.ErroOperacional("Usuário já está cadastrado nesta conta!"));

                convite.AceitaConvite(request.aceito);

                _unitOfWork.convitesRepostorio.Atualiza(convite);
                if (!request.aceito)
                {
                    await _unitOfWork.Commit();
                    return Resultado<RetornaPostCadastro>.GeraSucesso(new RetornaPostCadastro(request.aceito, null));
                }


                Conta? conta = await _unitOfWork.contasRepositorio.BuscarObjetoUnico(x => x.Id == convite.IdConta);

                var contaUsuario = new ContasUsuarios(conta!, convite.IdUsuarioDestinatario, convite.Acesso, null);

                contaUsuario = await _unitOfWork.contasUsuariosRepositorio.Adicionar(contaUsuario);

                _unitOfWork.convitesRepostorio.Atualiza(convite);

                await _unitOfWork.Commit();
                return Resultado<RetornaPostCadastro>.GeraSucesso(ContasUsuariosMapper.ParaDTO(request.aceito, contaUsuario));
            }
            catch (ContasUsuariosValidacao contasUsuariosExcessao)
            {
                return Resultado<RetornaPostCadastro>.GeraFalha(Falha.ErroOperacional(contasUsuariosExcessao.Message));
            }
        }
    }
}
