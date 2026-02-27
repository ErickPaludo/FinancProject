using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
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
    public class CriarContaHandler : IRequestHandler<CriarContaCommand, Resultado<RetornaContasDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuariosServico;

        public CriarContaHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuariosServico)
        {
            _unitOfWork = unitOfWork;
            _usuariosServico = usuariosServico;
        }
        public async Task<Resultado<RetornaContasDTO>> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Conta conta = new Conta(request.Titulo);
                Usuario usuario = await _usuariosServico.ObtemUsuario(request.IdUsuario);
                ContasUsuarios contaUsuario = new ContasUsuarios(conta, usuario);
               // conta.ContasUsuariosVinculados!.Add(contaUsuario);

                await _unitOfWork.contasUsuariosRepositorio.Adicionar(contaUsuario); //Cria a conta e a conta usuario pois os objetos estão linkados
              //  conta.AddUsuario(contaUsuario);
                await _unitOfWork.Commit();

                return Resultado<RetornaContasDTO>.GeraSucesso(ContaMapper.ParaDTO(conta));
            }
            catch (ContasValidacao contasExecao)
            {
                return Resultado<RetornaContasDTO>.GeraFalha(Falha.ErroOperacional(contasExecao.Message));
            }
            catch (ContasUsuariosValidacao contasUseuariosExcessao)
            {
                return Resultado<RetornaContasDTO>.GeraFalha(Falha.ErroOperacional(contasUseuariosExcessao.Message));
            }
        }
    }
}
