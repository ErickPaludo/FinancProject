using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Convites_.Commands;
using Financ.Application.DTOs.Convites.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Convites_.Handler
{
    public class CriaConviteHandler : IRequestHandler<CriaConviteCommand, Resultado<GetCriaConviteDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CriaConviteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<GetCriaConviteDTO>> Handle(CriaConviteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Usuario? usuario = await _unitOfWork.usuariosRepostorio.BuscarObjetoUnico(x => x.Email == request.emailDestinatario!);
                if (usuario is null)
                       return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Usuário destinatário do convite não existe."));
                
                var conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.idConta);

                if (conta is null)
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));

                var contaUsuario = await _unitOfWork.contasUsuariosRepositorio.BuscarPorCondicao(x => x.IdConta == request.idConta );            

                var contaUsuarioRemetente = contaUsuario.FirstOrDefault(x => x.IdUsuario == request.idRemetente);
                if (contaUsuarioRemetente is null)
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Você não está associado a está conta."));
                
                bool usuarioAssociado = contaUsuario.Any(x => x.IdUsuario == usuario.Id);
                if (usuarioAssociado)
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.ErroOperacional("Usuário já pertence a está conta"));  

              
                

               

                Convites convite = new Convites(request.acesso, contaUsuarioRemetente, "");
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
