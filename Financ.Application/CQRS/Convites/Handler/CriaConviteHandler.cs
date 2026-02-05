using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.DTOs.Convites.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
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
    public class CriaConviteHandler : IRequestHandler<CriaConviteCommand, Resultado<GetCriaConviteDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuarioIdentity;
        public CriaConviteHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuarioIdentity)
        {
            _unitOfWork = unitOfWork;
            _usuarioIdentity = usuarioIdentity;
        }
        public async Task<Resultado<GetCriaConviteDTO>> Handle(CriaConviteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var idUsuarioDestinatario = await _usuarioIdentity.ObtemIdUsuario(request.emailDestinatario!);
                if (string.IsNullOrEmpty(idUsuarioDestinatario))
                {
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Usuário destinatário do convite não cadastrado no sistema."));
                }

                Conta? conta = await _unitOfWork.contasRepositorio.BuscarObjetoUnico(x => x.Id == request.idConta);
                if (conta is null)
                {
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));
                }

                ContasUsuarios? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.BuscarObjetoUnico(x => x.IdConta == request.idConta && x.IdUsuario == request.idRemetente);
                if (contaUsuario is null)
                {
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Usuário remetente do convite não está associado a conta."));
                }

                if (await _unitOfWork.contasUsuariosRepositorio.BuscarObjetoUnico(x => x.IdUsuario == idUsuarioDestinatario && x.IdConta == request.idConta) is not null)
                {
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.ErroOperacional("Usuário destinataio já pertente a está conta."));
                }
                //Verifica se já existe um convite, que não tenha sido reprovado e que que não esteja expirado
                if (( await _unitOfWork.convitesRepostorio.BuscarPorCondicao(
                x => x.IdConta == request.idConta &&
                x.IdUsuarioDestinatario == idUsuarioDestinatario &&
                x.IdUsuarioRemetente == request.idRemetente &&
                DateTime.Now <= x.Expiracao &&
                x.Aceito != false)).Count() > 0)
                {
                   return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.ErroOperacional("Já existe um convite em andamento, aguarde o retorno do usuário destinatário."));
                }
                Usuario usuarioDestinatario = await _usuarioIdentity.ObtemUsuario(idUsuarioDestinatario);
                Convites convite = new Convites(contaUsuario, usuarioDestinatario, conta, request.acesso);
                await _unitOfWork.convitesRepostorio.Adicionar(convite);
                await _unitOfWork.Commit();

                return Resultado<GetCriaConviteDTO>.GeraSucesso(ConvitesMapper.ParaDTO(convite));
            }
            catch (ConvitesValidacao ex)
            {
                return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
