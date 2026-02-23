using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Querys;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.Contas.Get.Filtros;
using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Handler
{
    public class RetornaContasHandler : IRequestHandler<RetornaContaQuery, Resultado<List<RetornaContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RetornaContasHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<List<RetornaContasDTO>>> Handle(RetornaContaQuery request, CancellationToken cancellationToken)
        {
           var contasUsuarios = await ContasUsuariosSelecionadas(request);

            if (contasUsuarios.Count() == 0)
                return Resultado<List<RetornaContasDTO>>.GeraFalha(Falha.NaoEncontrado("Nenhuma conta foi encontrada!"));

            List<RetornaContasDTO> listaContas = new List<RetornaContasDTO>();
            foreach (var conta in contasUsuarios)
            {
                if (conta.Contas is null)
                {
                    return Resultado<List<RetornaContasDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada!"));
                }
                listaContas.Add(new RetornaContasDTO(conta.Contas.Id, conta.Contas.Titulo!, conta.Contas.Status));
            }

            return Resultado<List<RetornaContasDTO>>.GeraSucesso(listaContas);
        }
        private async Task<IEnumerable<ContasUsuarios>> ContasUsuariosSelecionadas(RetornaContaQuery filtros)
        {
            var filtroId = filtros.Filtros?.Id;
            var filtroTitulo = filtros.Filtros?.Titulo;
            var filtroStatus = filtros.Filtros?.Status;
            var possuiFiltros = filtros.Filtros != null;

            var contasUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContasDoUsuario(
                x => x.IdUsuario == filtros.IdUsuario && x.Contas!.Status != TiposStatusContas.Deletado
                && (!possuiFiltros || (
                    (!filtroId.HasValue || x.IdConta == filtroId.Value) &&
                    (string.IsNullOrEmpty(filtroTitulo) || x.Contas!.Titulo!.Contains(filtroTitulo)) &&
                    (!filtroStatus.HasValue || x.Contas!.Status == filtroStatus.Value)))
            );
            return contasUsuario;
        }
    }
}
